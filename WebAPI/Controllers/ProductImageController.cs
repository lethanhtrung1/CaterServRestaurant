using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class ProductImageController : ControllerBase {
		private readonly IProductImageService _service;

		public ProductImageController(IProductImageService service) {
			_service = service;
		}

		[HttpPost]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Update([FromForm] CreateProductImageRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.CreateAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Delete(Guid id) {
			var result = await _service.DeleteAsync(id);
			if (result) return Ok("Deleted successfully");
			return BadRequest("Internal server error occurred");
		}

		[HttpDelete("all/{productId:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> DeleteAll(Guid productId) {
			var result = await _service.DeleteRangeAsync(productId);
			if (result) return Ok("Deleted successfully");
			return BadRequest("Internal server error occurred");
		}

	}
}
