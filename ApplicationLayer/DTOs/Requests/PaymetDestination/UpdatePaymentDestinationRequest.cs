namespace ApplicationLayer.DTOs.Requests.PaymetDestination {
	public class UpdatePaymentDestinationRequest {
		public Guid Id { get; set; }
		public string? DesName { get; set; } = string.Empty;
		public string? DesShortName { get; set; } = string.Empty;
		public string? DesLogo { get; set; } = string.Empty;
		public int SortIndex { get; set; }
	}
}
