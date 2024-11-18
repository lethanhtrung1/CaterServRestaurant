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

		[HttpGet]
		public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request) {
			return Ok();
		}

		[HttpGet("vn-return")]
		public async Task<IActionResult> VnpayReturn() {
			return Ok();
		}
	}
}
