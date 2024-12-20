using ApplicationLayer.Common.Constants;
using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Email;
using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.DTOs.Responses.Account;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

				// Check email is confirmed
				if (!await _userManager.IsEmailConfirmedAsync(user)) {
					return new AuthResponseDto(false, "Email is not confirmed");
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

						return new AuthResponseDto(false, "Your account is locked due to multiple failed login attempts. Please try again in 5 minutes.");
					}
					return new AuthResponseDto(false, "Your email or password is incorrect, please try again");
				}

				// Check 2-FA
				if (await _userManager.GetTwoFactorEnabledAsync(user)) {
					// Generate OTP 2FA
					return await GenerateOTPFor2Factor(user);
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

				// Link the external login to the user
				var loginInfo = new UserLoginInfo("System", "", "System");
				await _userManager.AddLoginAsync(newUser, loginInfo);

				// Generate email confirmation code
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
				var message = new Message(newUser.Email, "Verify email", code);
				_emailService.SendEmail(message);

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

		public async Task<AuthResponseDto> VerifyTwoFactor(string email, string code) {
			var user = await _userManager.FindByEmailAsync(email);

			if (user is null) {
				return new AuthResponseDto(false, "Request failed. Please verify your credentials");
			}

			var providers = _userManager.GetValidTwoFactorProvidersAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;

			var isConfirm = await _userManager.VerifyTwoFactorTokenAsync(user, providers, code);
			//var isConfirm = await _userManager.VerifyTwoFactorTokenAsync(user, "email", code);
			if (!isConfirm) {
				return new AuthResponseDto(false, "Login failed. Please verify your credentials");
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
		}

		private async Task<AuthResponseDto> GenerateOTPFor2Factor(ApplicationUser user) {
			var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);

			if (!providers.Contains("Email")) {
				return new AuthResponseDto() {
					Success = false,
					Message = "Invalid 2-Factor Provider"
				};
			}

			var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
			var message = new Message(user.Email!, "Authentication token", token);
			_emailService.SendEmail(message);

			return new AuthResponseDto {
				Success = true,
				Message = "Code has been sent to your email, please check your email."
			};
		}

		public async Task<GeneralResponse> ManageTwoFactor(string email) {
			var user = await _userManager.FindByEmailAsync(email);
			if (user is null) {
				return new GeneralResponse(false, "Invalid request");
			}

			bool isTwoFactorEnabled = user.TwoFactorEnabled;

			await _userManager.SetTwoFactorEnabledAsync(user, !isTwoFactorEnabled);

			return new GeneralResponse(true, "Change two factor successfully");
		}

		public async Task<AuthResponseDto> HandleExternalLoginProviderCallBack(AuthenticateResult authenticateResult) {
			try {
				if (authenticateResult?.Principal == null) {
					throw new ArgumentException(nameof(authenticateResult.Principal), "Principal cannot be null");
				}

				var principal = authenticateResult.Principal;
				var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
				var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
				var providerKey = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier); // Use this for provider login info
				var provider = authenticateResult.Properties.Items["LoginProvider"]; // Get the provider name

				var existedUser = await _userManager.FindByEmailAsync(email!);
				var user = new ApplicationUser();

				if (existedUser == null) {
					user = new ApplicationUser {
						Name = email,
						Email = email,
						UserName = email,
						EmailConfirmed = true
					};
					// Create user without a password
					var result = await _userManager.CreateAsync(user);
					if (!result.Succeeded) {
						var errors = string.Join(", ", result.Errors.Select(e => e.Description));
						throw new ArgumentException("User creation failed: " + errors);
					}

					// Add role for user
					IdentityResult assignRoleResult = await _userManager.AddToRoleAsync(user, Role.CUSTOMER);
					if (!assignRoleResult.Succeeded) {
						var errors = string.Join(", ", result.Errors.Select(e => e.Description));
						throw new ArgumentException("User creation failed: " + errors);
					}

					// Link the external login to the user
					var loginInfo = new UserLoginInfo(provider, providerKey, provider);
					await _userManager.AddLoginAsync(existedUser ?? user, loginInfo);
				}

				// Check link external login
				var loginInfos = await _userManager.GetLoginsAsync(existedUser ?? user);
				var hasLinkedProvider = loginInfos.Any(login => login.LoginProvider == provider);
				if (!hasLinkedProvider) {
					throw new ApplicationException("User exists but has not linked this provider");
				}

				var token = await _tokenService.GenerateToken(user, populateExp: true);

				// Set token inside cookie
				var tokenRequestDto = new TokenRequestDto {
					AccessToken = token.AccessToken,
					RefreshToken = token.RefreshToken,
				};
				_tokenService.SetTokensInsideCookie(tokenRequestDto);

				return new AuthResponseDto {
					Success = true,
					AccessToken = token.AccessToken,
					RefreshToken = token.RefreshToken,
					Message = "Login successfully"
				};
			} catch (Exception ex) {
				throw new Exception($"Error handling external login: {ex.Message}");
			}
		}
	}
}
