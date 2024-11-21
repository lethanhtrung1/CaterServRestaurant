using ApplicationLayer.DTOs.Requests.Payment;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/payment")]
	[ApiController]
	public class PaymentController : ControllerBase {
		private readonly IPaymentService _service;

		public PaymentController(IPaymentService service) {
			_service = service;
		}

		//[HttpPost]
		//public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request) {
		//	var result = await _service.CreatePayment(request);
		//	if (result == null || !result.Success) {
		//		return BadRequest(result);
		//	}
		//	return Ok(result);
		//}

		[HttpPost("payment-url")]
		public IActionResult CreatePaymentUrl([FromBody] CreatePaymentRequest request) {
			var result = _service.CreatePaymentUrl(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("vnpay-return")]
		public IActionResult VnpayReturn() {
			return Ok("vnpay-return");
		}
	}
}
