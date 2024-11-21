using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Requests.UserProfile;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/user-profile")]
	[ApiController]
	public class UserProfileController : ControllerBase {
		private readonly IUserProfileService _service;

		public UserProfileController(IUserProfileService service) {
			_service = service;
		}

		[Authorize]
		[HttpGet("userId")]
		public async Task<IActionResult> GetByUserId(string userId) {
			var result = await _service.GetByUserId(userId);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[Authorize]
		[HttpPost("customer")]
		public async Task<IActionResult> UpSert([FromForm] CreateOrUpdateUserProfileRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.UpSertCustomer(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("staff")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> UpSertStaff([FromForm] CreateOrUpdateStaffProfileRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.UpSertStaff(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
