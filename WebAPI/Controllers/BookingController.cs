using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Booking;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/booking")]
	[ApiController]
	public class BookingController : ControllerBase {
		private readonly IBookingService _bookingService;

		public BookingController(IBookingService bookingService) {
			_bookingService = bookingService;
		}

		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> GetById(Guid id) {
			var result = await _bookingService.GetAsync(id);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet]
		public async Task<IActionResult> GetList([FromQuery] PagingRequest request) {
			if (request == null) {
				return BadRequest("Invalid client request");
			}
			var result = await _bookingService.GetListAsync(request);
			if (request is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> Add([FromBody] CreateBookingRequest request) {
			if (request is null) {
				return BadRequest("Invalid client request");
			}
			var result = await _bookingService.CreateAsync(request);
			if (result is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}


		[HttpPost("staff")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> AddForStaff([FromBody] CreateBookingRequest request) {
			if (request is null) {
				return BadRequest("Invalid client request");
			}
			var result = await _bookingService.CreateForStaffAsync(request);
			if (result is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] UpdateBookingRequest request) {
			if (request is null) {
				return BadRequest("Invalid client request");
			}
			var result = await _bookingService.UpdateAsync(request);
			if (result is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("change-table")]
		public async Task<IActionResult> ChangeTable([FromBody] ChangeTableRequest request) {
			if (request is null) {
				return BadRequest("Invalid client request");
			}
			var result = await _bookingService.ChangeTableAsync(request);
			if (result is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("status")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusBookingRequest request) {
			if (request is null) {
				return BadRequest("Invalid client request");
			}
			var result = await _bookingService.UpdateStatusAsync(request);
			if (result is null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		[Authorize(Roles = $"{Role.ADMIN},{Role.STAFF}")]
		public async Task<IActionResult> Delete(Guid id) {
			var result = await _bookingService.DeleteAsync(id);
			if (!result) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("cancel/id:{Guid}")]
		public async Task<IActionResult> CancelBooking(Guid id) {
			var result = await _bookingService.CancelBookingAsync(id);
			if (!result.Success) {
				return BadRequest(result);
			}
			return Ok(result);	
		}
	}
}
