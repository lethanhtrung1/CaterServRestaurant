namespace ApplicationLayer.DTOs.Requests.PaymetDestination {
	public class SetActivePaymentDestinationRequest {
		public Guid Id { get; set; }
		public bool IsActive { get; set; }
	}
}
