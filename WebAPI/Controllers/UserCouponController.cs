using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/user-coupon")]
	[ApiController]
	public class UserCouponController : ControllerBase {
		private readonly IUserCouponService _userCouponService;

		public UserCouponController(IUserCouponService userCouponService) {
			_userCouponService = userCouponService;
		}

        [Authorize]
		[HttpGet("get-all-user-coupon")]
		public async Task<IActionResult> GetAllCouponByUserId() {
			var result = await _userCouponService.GetAllCouponByUserId();
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

        [Authorize]
		[HttpPost("receive-coupon")]
		public async Task<IActionResult> CreateUserCoupon(Guid couponId) {
			var result = await _userCouponService.CreateUserCoupon(couponId);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

        [Authorize]
        [HttpDelete("{id:Guid}")]
		public async Task<IActionResult> RemoveUserCoupon(Guid id) {
			var result = await _userCouponService.RemoveUserCoupon(id);
			return result ? Ok(result) : BadRequest(result);
		}
	}
}
