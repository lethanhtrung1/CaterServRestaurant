namespace ApplicationLayer.DTOs.Responses.UserCoupon {
	public class UserCouponResponse {
		public string UserId { get; set; }
		public Guid CouponId { get; set; }
		public string CouponCode { get; set; }
		public decimal DiscountPercent { get; set; }
		public decimal DiscountAmount { get; set; }
	}
}
