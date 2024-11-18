namespace DomainLayer.Entites {
	public class UserCoupon : BaseEntity {
		public Guid CouponId { get; set; }
		public string UserId { get; set; }
		public Coupon Coupon { get; set; }
		public UserProfile User { get; set; }
	}
}
