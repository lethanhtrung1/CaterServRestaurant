namespace ApplicationLayer.DTOs.Responses.Booking {
	public class BookingTableResponse {
		public Guid TableId { get; set; }
		public string? Name { get; set; }
		public string? AreaName { get; set; }
	}
}
