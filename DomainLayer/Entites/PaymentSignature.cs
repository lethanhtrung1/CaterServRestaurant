namespace DomainLayer.Entites {
	public class PaymentSignature : BaseEntity {
		public Guid? PaymentId { get; set; }
		public string? SignValue { get; set; }
		public string? SignAlgo { get; set; }
		public string? SignOwn { get; set; }
		public DateTime? SignDate { get; set; }
		public bool IsValid { get; set; }

		public virtual Payment? Payment { get; set; }
	}
}
