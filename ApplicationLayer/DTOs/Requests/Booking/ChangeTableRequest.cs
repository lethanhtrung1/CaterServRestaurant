namespace ApplicationLayer.DTOs.Requests.Booking {
	public class ChangeTableRequest {
		public Guid BookingId { get; set; }
		public List<Guid> TableIds { get; set; }
	}
}
