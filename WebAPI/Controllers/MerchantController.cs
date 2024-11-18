using ApplicationLayer.DTOs.Requests.Merchant;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/merchant")]
	[ApiController]
	public class MerchantController : ControllerBase {
		private readonly IMerchantService _service;

		public MerchantController(IMerchantService service) {
			_service = service;
		}

		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> GetById([FromQuery] Guid id) {
			var result = await _service.GetById(id);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("paging")]
		public async Task<IActionResult> GetPaging([FromQuery] GetMerchantPaging request) {
			var result = await _service.GetPaging(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateMerchantRequest request) {
			var result = await _service.CreateMerchant(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] UpdateMerchantRequest request) {
			var result = await _service.UpdateMerchant(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> Delete([FromQuery] Guid id) {
			var result = await _service.Delete(id);
			if (result) {
				return Ok(result);
			}
			return BadRequest(result);
		}
	}
}
