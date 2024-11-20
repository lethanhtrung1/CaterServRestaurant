using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentSignatureController : ControllerBase {
		private readonly IPaymentSignatureService _service;

		public PaymentSignatureController(IPaymentSignatureService service) {
			_service = service;
		}

		//[HttpGet]
		//public IActionResult Get(string criteria) {
		//	return Ok();
		//}

		[HttpGet("paging")]
		public async Task<IActionResult> GetPaging([FromQuery] PagingRequest query) {
			var result = await _service.GetPaging(query);
			if (!result.Success) return BadRequest(result);
			return Ok(result);
		}

		[HttpGet("{id::Guid}")]
		public async Task<IActionResult> GetById([FromRoute] Guid id) {
			var result = await _service.GetById(id);
			if (!result.Success) return BadRequest(result);
			return Ok(result);
		}
	}
}
