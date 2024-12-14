using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Email;
using ApplicationLayer.DTOs.Requests.Payment;
using ApplicationLayer.Momo.Requests;
using ApplicationLayer.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebAPI.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase {
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;
		private readonly IEmailService _emailService;
		private readonly MomoOptions _momoOptions;

		public WeatherForecastController(ILogger<WeatherForecastController> logger, IEmailService emailService, IOptions<MomoOptions> momoOptions) {
			_logger = logger;
			_emailService = emailService;
			_momoOptions = momoOptions.Value;
		}

		//[Authorize]
		[HttpGet(Name = "GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get() {
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}

		[HttpGet("test-send-email")]
		public IActionResult SendEmail() {
			var message = new Message("trunglt1011@gmail.com", "Test", "Test send email");
			_emailService.SendEmail(message);
			return Ok();
		}


		/// <summary>
		/// Test payment momo url
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/// /// <remarks>
		///     POST /test-payment-momo-url
		///     {
		///			"paymentContent": "string",
		///			"paymentCurrency": "vnd",
		///			"requiredAmount": 100000,
		///			"paymentLanguage": "vn",
		///			"orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa1",
		///			"merchantId": "304c7956-ae94-4457-0083-08dd0a0a3694",
		///			"paymentDestinationId": "ad0537f4-e3da-45a3-414f-08dd0fcdffb9",
		///			"paymentDesname": "MOMO"
		///		}
		/// </remarks>
		[HttpPost("test-payment-momo-url")]
		public IActionResult TestMomo([FromBody] CreatePaymentRequest request) {
			string paymentUrl = string.Empty;
			var id = Guid.NewGuid();

			var momoPaymentRequest = new MomoPaymentRequest(
							_momoOptions.PartnerCode,
							id.ToString(),
							(long)request.RequiredAmount!,
							id.ToString(),
							request.PaymentContent ?? string.Empty,
							_momoOptions.ReturnUrl,
							_momoOptions.IpnUrl,
							"captureWallet",
							string.Empty
						);

			momoPaymentRequest.MakeSignature(_momoOptions.AccessKey, _momoOptions.SecretKey);

			(bool createMomoLinkResult, string? createMessage) = momoPaymentRequest.GetLink(_momoOptions.PaymentUrl);

			if (createMomoLinkResult) {
				paymentUrl = createMessage!;
			}

			return Ok(paymentUrl);
		}
	}
}
