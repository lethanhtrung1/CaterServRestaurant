using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Requests.Order;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/order")]
	[ApiController]
	public class OrderController : ControllerBase {
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService) {
			_orderService = orderService;
		}

		[HttpGet("{orderId:Guid}")]
		public async Task<IActionResult> GetByOrderId(Guid orderId) {
			var result = await _orderService.GetOrderById(orderId);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("staff/{bookingId:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> GetOrderByBooking(Guid bookingId) {
			var result = await _orderService.GetOrderByBooking(bookingId);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("get-orders-paging")]
		public async Task<IActionResult> GetOrdersPaging([FromQuery] GetOrdersPagingRequest request) {
			var result = await _orderService.GetOrders(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("get-orders-paging/{userId}")]
		public async Task<IActionResult> GetOrdersPaging([FromQuery] string userId, [FromQuery] GetOrdersPagingRequest request) {
			var result = await _orderService.GetOrdersByUserId(userId, request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request) {
			var result = await _orderService.CreateOrder(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("cancel/{orderId:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> CancelOrder(Guid orderId) {
			var result = await _orderService.CancelOrder(orderId);
			if (!result) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("status")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> UpdateStatus([FromBody] UpdateOrderStatusRequest request) {
			var result = await _orderService.UpdateOrderStatus(request);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost("staff/order")]
		public async Task<IActionResult> CreateOrderStaff(Guid BookingId) {
			var result = await _orderService.CreateOrderByBooking(BookingId);

			if (result == null || !result.Success) {
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpPost("staff/order-detail")]
		public async Task<IActionResult> AddOrderDetail([FromBody] CreateOrderDetailRequest request) {
			var result = await _orderService.AddOrderOrderDetail(request);

			if (result ==  null || !result.Success) {
				return BadRequest(result);
			}

			return Ok(result);
		}
	}
}
