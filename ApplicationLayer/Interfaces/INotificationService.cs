using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Notification;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Notification;

namespace ApplicationLayer.Interfaces {
	public interface INotificationService {
		Task<ApiResponse<NotificationDto>> GetByCustomerId(Guid id);
		Task<ApiResponse<PagedList<NotificationDto>>> GetNotificationPaging(GetNotificationRequest request);
		Task CreateNotification(string userId, string title, string description, string content);
		Task<bool> UpdateStatusSeenById(Guid id);
		Task<bool> DeleteNotification(Guid id);
	}
}
