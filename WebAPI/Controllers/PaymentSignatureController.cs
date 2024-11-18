using ApplicationLayer.DTOs.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentSignatureController : ControllerBase {
		public PaymentSignatureController() {

		}

		[HttpGet]
		public IActionResult Get(string criteria) {
			return Ok();
		}

		[HttpGet("paging")]
		public IActionResult GetPaging([FromQuery] PagingRequest query) {
			return Ok();
		}

		[HttpGet("{id::Guid}")]
		public IActionResult GetOne([FromRoute] string id) {
			return Ok();
		}
	}
}
