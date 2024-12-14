using Microsoft.AspNetCore.SignalR;

namespace ApplicationLayer.Hubs {
	public class DashboardHub : Hub {
		public override async Task OnConnectedAsync() {
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception? exception) {
			await base.OnDisconnectedAsync(exception);
		}
	}
}
