using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Notification;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Notification;
using ApplicationLayer.Hubs;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.SignalR;

namespace ApplicationLayer.Services {
	public class NotificationService : INotificationService {
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IHubContext<NotificationHub> _hubContext;
		private readonly ICurrentUserService _currentUserService;

		public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext,
			IMapper mapper, ICurrentUserService currentUserService) {
			_hubContext = hubContext;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_currentUserService = currentUserService;
		}

		public async Task CreateNotification(string userId, string title, string description, string content) {
			var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
			var notification = new Notification {
				Id = Guid.NewGuid(),
				Title = title,
				Description = description,
				Content = content,
				CreatedDate = DateTime.Now,
				Status = false, // send
				IsDelete = false,
				//UserId = staffId,
				UserName = user.UserName!,
			};

			await _unitOfWork.Notification.AddAsync(notification);
			await _unitOfWork.SaveChangeAsync();

			await _hubContext.Clients.User(notification.UserName).SendAsync("ReceivedNotification", notification.Title, notification.UserName);
		}

		public async Task<bool> DeleteNotification(Guid id) {
			var notification = await _unitOfWork.Notification.GetAsync(x => x.Id == id && !x.IsDelete);
			if (notification == null) return false;
			notification.IsDelete = true;
			await _unitOfWork.Notification.UpdateAsync(notification);
			await _unitOfWork.SaveChangeAsync();
			return true;
		}

		public async Task<ApiResponse<NotificationDto>> GetByCustomerId(Guid id) {
			var notification = await _unitOfWork.Notification.GetAsync(x => x.Id == id && !x.IsDelete);
			if (notification == null) {
				return new ApiResponse<NotificationDto>(false, "Notification not fount");
			}
			var response = _mapper.Map<NotificationDto>(notification);
			return new ApiResponse<NotificationDto>(response, true, "Retrieve noification successfully");
		}

		public async Task<ApiResponse<PagedList<NotificationDto>>> GetNotificationPaging(GetNotificationRequest request) {
			var username = _currentUserService.UserName;
			var notifications = await _unitOfWork.Notification.GetListAsync(x => x.UserName == username && !x.IsDelete);

			if (notifications == null || !notifications.Any()) {
				return new ApiResponse<PagedList<NotificationDto>>(false, "No record available");
			}

			int totalRecord = notifications.Count();
			var result = new List<NotificationDto>();

			if (request.IsPaging) {
				var notiPaging = notifications.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
				result = _mapper.Map<List<NotificationDto>>(notiPaging);
			} else {
				result = _mapper.Map<List<NotificationDto>>(notifications);
			}

			return new ApiResponse<PagedList<NotificationDto>>(
				new PagedList<NotificationDto>(result, request.PageNumber, request.PageSize, totalRecord),
				true, "Retrieve notifications successfully"
			);
		}

		public async Task<bool> UpdateStatusSeenById(Guid id) {
			var notification = await _unitOfWork.Notification.GetAsync(x => x.Id == id && !x.IsDelete);
			if (notification == null) return false;

			notification.Status = true;
			notification.SendTime = DateTime.UtcNow;
			notification.UpdatedDate = DateTime.UtcNow;

			await _unitOfWork.Notification.UpdateAsync(notification);
			await _unitOfWork.SaveChangeAsync();

			return true;
		}
	}
}
