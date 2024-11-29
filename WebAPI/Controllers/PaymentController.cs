using ApplicationLayer.DTOs.Requests.Payment;
using ApplicationLayer.DTOs.Responses.Payment;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Momo.Requests;
using ApplicationLayer.Utilities;
using ApplicationLayer.Vnpay.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/payment")]
	[ApiController]
	public class PaymentController : ControllerBase {
		private readonly IPaymentService _service;

		public PaymentController(IPaymentService service) {
			_service = service;
		}

		[HttpPost("payment-url")]
		public async Task<IActionResult> CreatePaymentUrl([FromBody] CreatePaymentRequest request) {
			//var result = _service.CreatePaymentUrl(request);
			var result = await _service.CreatePayment(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("vnpay-return")]
		public async Task<IActionResult> VnpayReturn([FromQuery] VnpayPayResponse request) {
			string returnUrl = string.Empty;
			var returnModel = new PaymentReturnDto();
			var processResult = await _service.PaymentReturn(request);

			if (processResult.Success) {
				returnModel = processResult.Data.Item1 as PaymentReturnDto;
				returnUrl = processResult.Data.Item2 as string;
			}

			if (returnUrl.EndsWith("/")) {
				returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
			}
			return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
			//return Redirect($"{returnUrl}");
		}

		[HttpGet("momo-return")]
		public async Task<IActionResult> MomoReturn([FromQuery] MomoPaymentResultRequest request) {
			string returnUrl = string.Empty;
			var returnModel = new PaymentReturnDto();
			var processResult = await _service.MomoPaymentReturn(request);

			if (processResult.Success) {
				returnModel = processResult.Data.Item1 as PaymentReturnDto;
				returnUrl = processResult.Data.Item2 as string;
			}

			if (returnUrl.EndsWith("/")) {
				returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
			}
			return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
		}
	}
}
