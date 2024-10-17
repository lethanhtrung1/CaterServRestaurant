using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase {
		private readonly IAuthService _authService;

		public AccountController(IAuthService authService) {
			_authService = authService;
		}

		[HttpPost("identity/login")]
		public async Task<IActionResult> Login(LoginRequestDto request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			return Ok(await _authService.Login(request));
		}

		[HttpPost("identity/register")]
		public async Task<IActionResult> Register(RegistrationRequestDto request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			return Ok(await _authService.Register(request));
		}
	}
}
