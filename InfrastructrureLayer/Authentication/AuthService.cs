using ApplicationLayer.Common.Constants;
using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Email;
using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.DTOs.Responses.Account;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Identity;

namespace InfrastructrureLayer.Authentication {
	public class AuthService : IAuthService {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly ILogException _logger;
		private readonly IEmailService _emailService;

		public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService,
			ILogException logger, IEmailService emailService) {
			_userManager = userManager;
			_tokenService = tokenService;
			_logger = logger;
			_emailService = emailService;
		}

		public async Task<AuthResponseDto> Login(LoginRequestDto request) {
			try {
				var user = await _userManager.FindByEmailAsync(request.Email);

				if (user is null) {
					return new AuthResponseDto(false, "Your email or password is incorrect, please try again");
				}

				if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow) {
					return new AuthResponseDto(false, "Your account is locked");
				}

				if (!await _userManager.CheckPasswordAsync(user, request.Password)) {
					// Increase failed accesses
					await _userManager.AccessFailedAsync(user);

					if (await _userManager.IsLockedOutAsync(user)) {
						//var content = $"Your account is locked out. If you want to reset the password, " +
						//	$"you can use the forgot password link on the login page.";

						//var message = new Message(request.Email!, "Locked out account information", content);
						//_emailSender.SendEmail(message);

						return new AuthResponseDto(false, "Your account is locked due to multiple failed login attempts");
					}
					return new AuthResponseDto(false, "Your email or password is incorrect, please try again");
				}

				var jwtToken = await _tokenService.GenerateToken(user, populateExp: true);
				
				// store refresh token as cookies
				var tokenDto = new TokenRequestDto {
					AccessToken = jwtToken.AccessToken,
					RefreshToken = jwtToken.RefreshToken,
				};
				// Set token to inside Cookie
				_tokenService.SetTokensInsideCookie(tokenDto);

				// Reset access failed
				await _userManager.ResetAccessFailedCountAsync(user);

				return new AuthResponseDto() {
					Success = true,
					Message = "Login successfully",
					AccessToken = jwtToken.AccessToken,
					RefreshToken = jwtToken.RefreshToken,
				};
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new AuthResponseDto(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<AuthResponseDto> RefreshToken(TokenRequestDto request) {
			try {
				var token = await _tokenService.RefreshToken(request);

				return new AuthResponseDto() {
					Success = true,
					Message = "Refresh Token successfully",
					AccessToken = token.AccessToken,
					RefreshToken = token.RefreshToken,
				};
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new AuthResponseDto(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<AuthResponseDto> Register(RegistrationRequestDto request) {
			try {
				var checkUserExist = await _userManager.FindByEmailAsync(request.Email);
				if (checkUserExist is not null) {
					return new AuthResponseDto(false, "Email already exist");
				}

				var newUser = new ApplicationUser {
					Name = request.Email,
					Email = request.Email,
					UserName = request.Email,
					PasswordHash = request.Password
				};

				var createUser = await _userManager.CreateAsync(newUser, request.Password);
				if (!createUser.Succeeded) {
					return new AuthResponseDto(false, "Error occured while creating account");
				}

				// Add role for user
				IdentityResult assignRoleResult = await _userManager.AddToRoleAsync(newUser, Role.CUSTOMER);
				if (!assignRoleResult.Succeeded) {
					return new AuthResponseDto(false, "Error occured while creating account");
				}

				return new AuthResponseDto(true, "Registration successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new AuthResponseDto(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<GeneralResponse> ChangePassword(ChangePasswordRequest request) {
			try {
				var user = await _userManager.FindByEmailAsync(request.Email);
				if (user == null) {
					return new GeneralResponse(false, "Request failed. Please verify your credentials");
				}

				// check current password
				if (!(await _userManager.CheckPasswordAsync(user, request.CurrentPassword))) {
					return new GeneralResponse(false, "Request failed. Please verify your credentials");
				}

				var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
				if (!result.Succeeded) {
					var errors = string.Join(", ", result.Errors.Select(e => e.Description));
					return new GeneralResponse(false, $"Change password failed: {errors}");
				}

				return new GeneralResponse(true, "Password has been changed successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new GeneralResponse(false, $"An unexpected error occurred. Please try again later.");
			}
		}

		public async Task<GeneralResponse> ForgotPassword(ForgotPasswordRequest request) {
			try {
				var user = await _userManager.FindByEmailAsync(request.Email);
				if (user == null) {
					return new GeneralResponse(false, "Request failed. Please verify your credentials");
				}

				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var message = new Message(user.Email!, "Reset password code", code);
				_emailService.SendEmail(message);

				return new GeneralResponse(true, "Code has been sent to your email, please check your email");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new GeneralResponse(false, $"An unexpected error occurred. Please try again later.");
			}
		}

		public async Task<GeneralResponse> ResetPassword(ResetPasswordRequest request) {
			try {
				var user = await _userManager.FindByEmailAsync(request.Email!);
				if (user == null) {
					return new GeneralResponse(false, "Request failed. Please verify your credentials");
				}

				// reset password
				var result = await _userManager.ResetPasswordAsync(user, request.Code!, request.Password!);
				if (!result.Succeeded) {
					var errors = result.Errors.Select(x => x.Description);
					return new GeneralResponse(false, errors.First());
				}

				// if account is locked out
				await _userManager.SetLockoutEndDateAsync(user, null);

				return new GeneralResponse(true, "Reset password successful");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new GeneralResponse(false, $"An unexpected error occurred. Please try again later.");
			}
		}

		public async Task<GeneralResponse> VerifyEmail(VerifyEmailRequest request) {
			try {
				if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code)) {
					return new GeneralResponse(false, "Request failed. Please verify your credentials");
				}

				var user = await _userManager.FindByEmailAsync(request.Email);
				if (user is null) {
					return new GeneralResponse(false, "Invalid identity provided");
				}

				var confirmResult = await _userManager.ConfirmEmailAsync(user, request.Code);
				if (!confirmResult.Succeeded) {
					return new GeneralResponse(false, "Invalid email confirmation request");
				}

				return new GeneralResponse(true, "Email confirmed successfully, you can proceed to login");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new GeneralResponse(false, $"An unexpected error occurred. Please try again later.");
			}
		}
	}
}
