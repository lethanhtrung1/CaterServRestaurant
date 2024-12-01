using ApplicationLayer.Common.Constants;
using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.User;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

		[HttpGet("current-user")]
		public IActionResult Get() {
			return Ok(_currentUserService.UserId);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = $"{Role.ADMIN}")]
		public async Task<IActionResult> GetById(string id) {
			var result = await _userService.GetById(id);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("paging")]
		[Authorize(Roles = $"{Role.ADMIN}")]
		public async Task<IActionResult> GetPaging([FromQuery] PagingRequest request) {
			var result = await _userService.GetPaging(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = $"{Role.ADMIN}")]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request) {
			var result = await _userService.CreateUser(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("ban-user/{id}")]
		[Authorize(Roles = $"{Role.ADMIN}")]
		public async Task<IActionResult> BanUser(string id) {
			var result = await _userService.BanUser(id);
			if (!result) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("unban/{id}")]
		[Authorize(Roles = $"{Role.ADMIN}")]
		public async Task<IActionResult> UnbanUser(string id) {
			var result = await _userService.UnbanUser(id);
			if (!result) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("roles")]
		[Authorize(Roles = $"{Role.ADMIN}")]
		public async Task<IActionResult> GetRoles() {
			return Ok(await _userService.GetRoles());
		}

		[HttpPost("change-role")]
		[Authorize(Roles = $"{Role.ADMIN}")]
		public async Task<IActionResult> ChangeRole(string userId, string roleName) {
			var result = await _userService.ChangeRole(userId, roleName);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
