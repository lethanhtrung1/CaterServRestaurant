using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class MenuController : ControllerBase {
		private readonly IMenuService _service;

		public MenuController(IMenuService service) {
			_service = service;
		}

		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> GetById(Guid id) {
			var result = await _service.GetAsync(id);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet]
		public async Task<IActionResult> GetList([FromQuery] PagingRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.GetListAsync(request);
			if (request is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Add([FromForm] CreateMenuRequest request) {
			if (request is null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.CreateAsync(request);
			if (result is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromForm] UpdateMenuRequest request) {
			if (request is null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.UpdateAsync(request);
			if (result is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("id:{Guid}")]
		public async Task<IActionResult> Delete(Guid id) {
			var result = await _service.DeleteAsync(id);
			if (!result) {
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
