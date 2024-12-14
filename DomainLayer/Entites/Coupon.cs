namespace DomainLayer.Entites {
	public class Coupon : BaseEntity {
		public string CouponCode { get; set; } // 2481ZKCH3Y
		public decimal DiscountPercent { get; set; }
		public decimal DiscountAmount { get; set; }
		public int Quantity { get; set; }
		public bool Inactive { get; set; }
		//public DateTime ApplyFromDate { get; set; }
		//public DateTime ApplyToDate { get; set;

		///// <summary>
		///// Loại giảm giá: 1 - Giảm giá theo %, 2 - Giảm giá theo số tiền
		///// </summary>
		//public int DiscountType { get; set; }
		public virtual IEnumerable<UserCoupon>? UserCoupons { get; set; }
	}
}
