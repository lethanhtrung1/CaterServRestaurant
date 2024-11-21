namespace ApplicationLayer.DTOs.Requests.Payment {
	public class CreatePaymentRequest {
		public string PaymentContent { get; set; } = string.Empty;
		public string PaymentCurrency { get; set; } = string.Empty;
		//public string PaymentRefId { get; set; } = string.Empty;
		public decimal? RequiredAmount { get; set; }
		//public DateTime? PaymentDate { get; set; } = DateTime.Now;
		//public DateTime? ExpireDate { get; set; } = DateTime.Now.AddMinutes(15);
		public string? PaymentLanguage { get; set; } = string.Empty;
		public Guid OrderId { get; set; }
		public Guid MerchantId { get; set; }
		public Guid PaymentDestinationId { get; set; }
		public string? PaymentDesname { get; set; } = "VNPAY";
		//public string? Signature { get; set; } = string.Empty;
	}
}
