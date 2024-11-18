using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/order-detail")]
	[ApiController]
	public class OrderDetailController : ControllerBase {
		private readonly IOrderDetailService _service;

		public OrderDetailController(IOrderDetailService service) {
			_service = service;
		}

		[HttpGet("{orderId:Guid}")]
		public async Task<IActionResult> GetOrderDetails(Guid orderId) {
			var result = await _service.GetOrderDetail(orderId);
			if (result.Success) {
				return Ok(result.Data);
			}
			return BadRequest(result);
		}

		[HttpGet("get-paging")]
		public async Task<IActionResult> GetOrderDetailsPaging([FromQuery] PagingRequest request) {
			var result = await _service.GetOrderDetailsPaging(request);
			if (result.Success) {
				return Ok(result.Data);
			}
			return BadRequest(result);
		}
	}
}
