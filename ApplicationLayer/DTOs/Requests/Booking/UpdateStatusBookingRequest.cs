namespace ApplicationLayer.DTOs.Requests.Booking {
	public class UpdateStatusBookingRequest {
		public Guid BookingId { get; set; }
		public string Status { get; set; }
	}
}
