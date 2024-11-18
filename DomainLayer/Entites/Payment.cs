namespace DomainLayer.Entites {
	public class Payment : BaseEntity {
		public Guid OrderId { get; set; }
		public string? PaymentContent { get; set; }
		public string? PaymentCurrency { get; set; }
		//public string PaymentRefId { get; set; }
		public decimal? RequiredAmount { get; set; }
		public DateTime PaymentDate { get; set; } = DateTime.Now;
		public DateTime ExpireDate { get; set; }
		public string? PaymentLanguage { get; set; }
		public Guid MerchantId { get; set; }
		public Guid PaymentDestinationId { get; set; }
		public decimal? PaidAmount { get; set; }
		public string? PaymentStatus { get; set; }
		public string? PaymentLastMessage { get; set; }

		public virtual Merchant Merchant { get; set; }
		public virtual PaymentDestination PaymentDestination { get; set; }
		public virtual IEnumerable<PaymentSignature> PaymentSignature { get; set; }
		public virtual Order Order { get; set; }
	}
}
