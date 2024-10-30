using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase {
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService) {
			_categoryService = categoryService;
		}

		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> GetById(Guid categoryId) {
			if (categoryId == Guid.Empty) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.GetByIdAsync(categoryId);
			if (result == null || !result.Success) {
				return BadRequest(result!.Message);
			}
			return Ok(result);
		}

		[HttpGet("{code}")]
		public async Task<IActionResult> GetByCode(string categoryCode) {
			if (categoryCode == string.Empty) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.GetByCodeAsync(categoryCode);
			if (result == null || !result.Success) {
				return BadRequest(result!.Message);
			}
			return Ok(result);
		}

		[HttpGet]
		public async Task<IActionResult> GetList([FromQuery] PagingRequest request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.GetListAsync(request);
			if (result == null || !result.Success) {
				return BadRequest(result!.Message);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Add([FromBody] CreateCategoryRequest request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.CreateAsync(request);
			if (result == null || !result.Success) {
				return BadRequest(result!.Message);
			}
			return Ok(result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request) {
			if (request == null) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.UpdateAsync(request);
			if (result == null || !result.Success) {
				return BadRequest(result!.Message);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> Delete(Guid categoryId) {
			if (categoryId == Guid.Empty) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.DeleteAsync(categoryId);
			if (!result) {
				return BadRequest("Internal server error occurred");
			}
			return Ok(result);
		}
	}
}
