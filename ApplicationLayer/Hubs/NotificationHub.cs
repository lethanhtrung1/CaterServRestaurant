using DomainLayer.Common;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ApplicationLayer.Hubs {
	public class NotificationHub : Hub {
		private readonly IUnitOfWork _unitOfWork;
		private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>();

		public NotificationHub(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public async Task SendNotification(string message, string userId) {
			string connectionId;
			lock (_connections) {
				_connections.TryGetValue(userId, out connectionId);
			}

			if (!string.IsNullOrEmpty(connectionId)) {
				await Clients.Client(connectionId).SendAsync("ReceivedNotification", message);
			}
		}

		public override async Task OnConnectedAsync() {
			await Clients.Caller.SendAsync("OnConnected");

			string userId = GetUserIdFromContext();

			if (!string.IsNullOrEmpty(userId)) {
				lock (_connections) {
					_connections[userId] = Context.ConnectionId; // Assign userId for ConnectionId
				}
			}

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception) {
			string userId = GetUserIdFromContext();

			if (!string.IsNullOrEmpty(userId)) {
				lock (_connections) {
					_connections.Remove(userId);
				}
			}

			await base.OnDisconnectedAsync(exception);
		}

		private string GetUserIdFromContext() {
			return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
		}
	}
}
