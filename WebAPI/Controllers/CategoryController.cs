using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
		public async Task<IActionResult> GetById(Guid id) {
			if (id == Guid.Empty) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.GetByIdAsync(id);
			if (result == null || !result.Success) {
				return BadRequest(result!.Message);
			}
			return Ok(result);
		}

		[HttpGet("{code}")]
		public async Task<IActionResult> GetByCode(string code) {
			if (code == string.Empty) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.GetByCodeAsync(code);
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
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
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
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
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
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Delete(Guid id) {
			if (id == Guid.Empty) {
				return BadRequest("Invalid request");
			}
			var result = await _categoryService.DeleteAsync(id);
			if (!result) {
				return BadRequest("Internal server error occurred");
			}
			return Ok(result);
		}
	}
}
