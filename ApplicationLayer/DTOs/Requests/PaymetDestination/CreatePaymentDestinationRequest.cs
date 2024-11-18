namespace ApplicationLayer.DTOs.Requests.PaymetDestination {
	public class CreatePaymentDestinationRequest {
		public string? DesName { get; set; } = string.Empty;
		public string? DesShortName { get; set; } = string.Empty;
		public string? DesParentId { get; set; } = string.Empty;
		public string? DesLogo { get; set; } = string.Empty;
		public int SortIndex { get; set; }
	}
}
