using ApplicationLayer.DTOs.Requests.PaymetDestination;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/payment-destination")]
	[ApiController]
	public class PaymentDestinationController : ControllerBase {
		private readonly IPaymentDestinationService _service;

		public PaymentDestinationController(IPaymentDestinationService service) {
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
		public async Task<IActionResult> GetPaging([FromQuery] GetPaymentDesPagingRequest request) {
			var result = await _service.GetPaging(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreatePaymentDestinationRequest request) {
			var result = await _service.Create(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] UpdatePaymentDestinationRequest request) {
			var result = await _service.Update(request);
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
