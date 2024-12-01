namespace ApplicationLayer.DTOs.Responses.Payment {
	public class PaymentResponse {
		public Guid Id { get; set; }
		public string? PaymentContent { get; set; }
		public string? PaymentCurrency { get; set; }
		public string? PaymentDes {  get; set; }
		public decimal RequiredAmount { get; set; }
		public DateTime PaymentDate { get; set; }
		public string? PaymentStatus { get; set; }
		public string? PaymentLastMessage { get; set; }
	}
}
