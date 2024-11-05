using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Coupon;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class CouponController : ControllerBase {
		private readonly ICouponService _service;

		public CouponController(ICouponService service) {
			_service = service;
		}

		[HttpGet("admin/{id:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> GetById(Guid id) {
			var result = await _service.GetByIdAsync(id);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("admin/{code}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> GetByCode(string code) {
			var result = await _service.GetByCodeAsync(code);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("admin/get-list")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> GetList([FromQuery] PagingRequest request) {
			var result = await _service.GetListAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> GetCouponActiveById(Guid id) {
			var result = await _service.GetCouponActiveByIdAsync(id);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("get-list")]
		public async Task<IActionResult> GetListCouponActive([FromQuery] PagingRequest request) {
			var result = await _service.GetListCouponActiveAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Add([FromBody] CreateCouponRequest request) {
			var result = await _service.CreateAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Update([FromBody] UpdateCouponRequest request) {
			var result = await _service.UpdateAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Delete(Guid id) {
			var result = await _service.DeleteAsync(id);
			return Ok(result);
		}
	}
}
