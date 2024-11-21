namespace ApplicationLayer.DTOs.Responses.Payment {
	public class PaymentLinkDto {
		public Guid PaymentId { get; set; }
		public string PaymentUrl { get; set; } = string.Empty;
	}
}
