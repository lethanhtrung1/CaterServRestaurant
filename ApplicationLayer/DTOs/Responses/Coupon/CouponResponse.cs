namespace ApplicationLayer.DTOs.Responses.Coupon {
	public class CouponResponse {
		public Guid Id { get; set; }
		public string? CouponCode { get; set; }
		public decimal DiscountPercent { get; set; }
		public decimal DiscountAmount { get; set; }
		public int Quantity { get; set; }
		public bool Inactive { get; set; }
	}
}
