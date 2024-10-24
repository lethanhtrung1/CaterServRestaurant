namespace DomainLayer.Entites {
	public class InvoiceCoupon : BaseEntity {
		public Guid InvoiceId { get; set; }
		public Guid CouponId { get; set; }
		/// <summary>
		/// Phần trăm giảm giá
		/// </summary>
		public decimal DiscountPercent { get; set; }
		/// <summary>
		/// Số tiền giảm giá
		/// </summary>
		public decimal DiscountAmount { get; set; }
		public DateTime ApplyFromDate { get; set; }
		public DateTime ApplyToDate { get; set; }
		public string? ApplyCondition { get; set; }

		public Invoice? Invoice { get; set; }
		public Coupon? Coupon { get; set; }
	}
}
