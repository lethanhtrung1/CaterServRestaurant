using ApplicationLayer.Common.Constants;
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

		public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, ILogException logger) {
			_userManager = userManager;
			_tokenService = tokenService;
			_logger = logger;
		}

		public async Task<AuthResponseDto> Login(LoginRequestDto request) {
			try {
				var user = await _userManager.FindByEmailAsync(request.Email);

				if (user is null) {
					return new AuthResponseDto(false, "Your email or password is incorrect, please try again");
				}

				if (!await _userManager.CheckPasswordAsync(user, request.Password)) {
					return new AuthResponseDto(false, "Your email or password is incorrect, please try again");
				}

				var jwtToken = await _tokenService.GenerateToken(user, populateExp: true);
				
				// store refresh token as cookies
				var tokenDto = new TokenRequestDto {
					AccessToken = jwtToken.AccessToken,
					RefreshToken = jwtToken.RefreshToken,
				};
				_tokenService.SetTokensInsideCookie(tokenDto);

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
	}
}
