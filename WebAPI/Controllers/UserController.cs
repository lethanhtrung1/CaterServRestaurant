using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.User;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/user")]
	[ApiController]
	public class UserController : ControllerBase {
		private readonly ICurrentUserService _currentUserService;
		private readonly IUserService _userService;

		public UserController(ICurrentUserService currentUserService, IUserService userService) {
			_currentUserService = currentUserService;
			_userService = userService;
		}

		[HttpGet("curent-user")]
		public IActionResult Get() {
			return Ok(_currentUserService.UserId);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(string id) {
			var result = await _userService.GetById(id);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("paging")]
		public async Task<IActionResult> GetPaging([FromQuery] PagingRequest request) {
			var result = await _userService.GetPaging(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request) {
			var result = await _userService.CreateUser(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("ban-user/{id}")]
		public async Task<IActionResult> BanUser(string id) {
			var result = await _userService.BanUser(id);
			if (!result) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("unban/{id}")]
		public async Task<IActionResult> UnbanUser(string id) {
			var result = await _userService.UnbanUser(id);
			if (!result) {
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
