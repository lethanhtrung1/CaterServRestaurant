using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase {
		private readonly IAuthService _authService;
		private readonly ITokenService _tokenService;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(IAuthService authService, ITokenService tokenService, SignInManager<ApplicationUser> signInManager) {
			_authService = authService;
			_tokenService = tokenService;
			_signInManager = signInManager;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequestDto request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			var response = await _authService.Login(request);
			if (response == null || !response.Success) {
				return BadRequest(response!.Message);
			}
			return Ok(response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegistrationRequestDto request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			var response = await _authService.Register(request);
			if (response == null || !response.Success) {
				return BadRequest(response!.Message);
			}
			return Ok(response);
		}

		[HttpPost("refresh-access-token")]
		public async Task<IActionResult> RefreshToken(TokenRequestDto request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}

			var response = await _authService.RefreshToken(request);

			if (!response.Success) {
				if (response.Message!.Contains("expired")) {
					return Unauthorized(response.Message);
				}
				return StatusCode(500, response.Message);
			}

			return Ok(response);
		}

		[Authorize]
		[HttpPost("refresh-token/revoke")]
		public async Task<IActionResult> RevokeRefreshToken(RevokeRefreshTokenRequestDto request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			await _tokenService.RevokeRefreshToken(request.UserId, request.RefreshToken);
			return NoContent();
		}

		[Authorize]
		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request) {
			var result = await _authService.ChangePassword(request);
			if (result == null || !result.IsSuccess) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request) {
			var result = await _authService.ForgotPassword(request);
			if (result == null || !result.IsSuccess) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request) {
			var result = await _authService.ResetPassword(request);
			if (result == null || !result.IsSuccess) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("verify-email")]
		public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request) {
			var result = await _authService.VerifyEmail(request);
			if (result == null || !result.IsSuccess) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[Authorize]
		[HttpPost("manage/2fa")]
		public async Task<IActionResult> ManageTwoFactor(string email) {
			var result = await _authService.ManageTwoFactor(email);
			if (result == null || !result.IsSuccess) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("signin-google")]
		public IActionResult SigninWithGoogle() {
			var redirectUrl = Url.Action("GoogleCallback", "Account", null, Request.Scheme);
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet("google/callback")]
		public async Task<IActionResult> GoogleCallback() {
			var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

			//var principal = authenticateResult.Principal;
			//var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
			//var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
			//var providerKey = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
			//var provider = authenticateResult.Properties.Items["LoginProvider"];

			//var tokens = authenticateResult.Properties.GetTokens();
			//return Ok(tokens);

			if (authenticateResult == null || !authenticateResult.Succeeded) {
				return BadRequest("external authentication error");
			}

			var response = await _authService.HandleExternalLoginProviderCallBack(authenticateResult);

			return Ok(response);
		}
	}
}
