namespace ApplicationLayer.DTOs.Requests.Coupon {
	public class CreateCouponRequest {
		public string? CouponCode { get; set; }
		public decimal DiscountPercent { get; set; }
		public decimal DiscountAmount { get; set; }
		public int Quantity { get; set; }
		public bool Inactive { get; set; }
	}
}
