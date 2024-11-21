using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Table;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class TableController : ControllerBase {
		private readonly ITableService _service;

		public TableController(ITableService service) {
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

		[HttpGet("get-all")]
		public async Task<IActionResult> GetAllPaging([FromQuery] PagingRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.GetListAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("filter")]
		public async Task<IActionResult> Filter([FromQuery] FilterTableRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.FilterAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet]
		public async Task<IActionResult> GetAvailable([FromQuery] PagingRequest request) {
			var result = await _service.GetTableAvailableAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Delete(Guid id) {
			return Ok(await _service.DeleteAsync(id));
		}

		[HttpPost]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Add(CreateTableRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.CreateAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Update(UpdateTableRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.UpdateAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("status")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> UpdateStatus(UpdateTableStatus request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _service.UpdateStatusAsync(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
