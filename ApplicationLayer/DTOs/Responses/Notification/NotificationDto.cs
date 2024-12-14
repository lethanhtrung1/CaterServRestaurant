namespace ApplicationLayer.DTOs.Responses.Notification {
	public class NotificationDto {
		public Guid Id { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? Content { get; set; }
		public DateTime SendTime { get; set; }
		public bool Status { get; set; }
	}
}
