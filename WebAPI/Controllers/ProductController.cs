using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase {
		private readonly IProductService _service;

		public ProductController(IProductService service) {
			_service = service;
		}

		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> GetById(Guid id) {
			var result = await _service.GetByIdAsync(id);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("get-list")]
		public async Task<IActionResult> GetList([FromQuery] PagingRequest request) {
			var result = await _service.GetListAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("filter")]
		public async Task<IActionResult> Filter([FromQuery] FilterProductRequest request) {
			var result = await _service.FilterAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Add([FromForm] CreateProductRequest request) {
			var result = await _service.CreateAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromForm] UpdateProductRequest request) {
			var result = await _service.UpdateAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> Delete(Guid id) {
			var result = await _service.DeleteAsync(id);
			return Ok(result);
		}
	}
}
