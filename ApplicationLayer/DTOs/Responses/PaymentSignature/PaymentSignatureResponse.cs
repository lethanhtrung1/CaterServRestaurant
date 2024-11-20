namespace ApplicationLayer.DTOs.Responses.PaymentSignature {
	public class PaymentSignatureResponse {
		public Guid Id { get; set; }
		public Guid? PaymentId { get; set; }
		public string? SignValue { get; set; } = string.Empty;
		public string? SignAlgo { get; set; } = string.Empty;
		public string? SignOwn { get; set; } = string.Empty;
		public DateTime? SignDate { get; set; }
		public bool IsValid { get; set; }
	}
}
