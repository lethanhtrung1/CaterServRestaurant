using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Notification {
	public class GetNotificationRequest : PagingRequest {
		public bool IsPaging { get; set; }
	}
}
