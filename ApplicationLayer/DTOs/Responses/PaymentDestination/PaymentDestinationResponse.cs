namespace ApplicationLayer.DTOs.Responses.PaymentDestination {
	public class PaymentDestinationResponse {
		public Guid Id { get; set; }
		public string? DesName { get; set; } = string.Empty;
		public string? DesShortName { get; set; } = string.Empty;
		public string? DesLogo { get; set; } = string.Empty;
		public int DesSortIndex { get; set; }
		public bool IsActive { get; set; }
	}
}
