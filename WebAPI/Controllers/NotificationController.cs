using ApplicationLayer.DTOs.Requests.Notification;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/notification")]
	[ApiController]
	public class NotificationController : ControllerBase {
		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService) {
			_notificationService = notificationService;
		}

		[Authorize]
		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> Get(Guid id) {
			var result = await _notificationService.GetByCustomerId(id);
			if (result == null || !result.Success) return BadRequest(result);
			return Ok(result);
		}

		[Authorize]
		[HttpGet("paging")]
		public async Task<IActionResult> GetPaging([FromQuery] GetNotificationRequest request) {
			var result = await _notificationService.GetNotificationPaging(request);
			if (result == null || !result.Success) return BadRequest(result);
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> Delete(Guid id) {
			var result = await _notificationService.DeleteNotification(id);
			if (!result) return BadRequest(result);
			return Ok(result);
		}

		[HttpPut("update-status-send/{id:Guid}")]
		public async Task<IActionResult> UpdateSend(Guid id) {
			var result = await _notificationService.UpdateStatusSeenById(id);
			if (!result) return BadRequest(result);
			return Ok(result);
		}
	}
}
