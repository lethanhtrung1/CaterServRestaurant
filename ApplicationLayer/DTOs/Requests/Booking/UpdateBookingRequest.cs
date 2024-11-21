namespace ApplicationLayer.DTOs.Requests.Booking {
	public class UpdateBookingRequest {
		public Guid Id { get; set; }
		public int PeopleCount { get; set; }
		//public string? Status { get; set; }
		public string? Notes { get; set; }
		//public DateTime BookingDate { get; set; }
		public DateTime CheckinTime { get; set; }
		public string? CustomerName { get; set; }
		public string? Phone { get; set; }
	}
}
