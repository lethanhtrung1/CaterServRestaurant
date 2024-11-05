namespace DomainLayer.Entites {
	public class Coupon : BaseEntity {
		public string CouponCode { get; set; }
		public decimal DiscountPercent { get; set; }
		public decimal DiscountAmount { get; set; }
		public int Quantity { get; set; }
		public bool Inactive { get; set; }
		public virtual IEnumerable<InvoiceCoupon>? InvoiceCoupons { get; set; }
	}
}
