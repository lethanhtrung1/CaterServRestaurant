namespace ApplicationLayer.DTOs.Requests.Order {
	public class UpdateOrderStatusRequest {
		public Guid Id { get; set; }
		public string Status { get; set; } = string.Empty;
	}
}
