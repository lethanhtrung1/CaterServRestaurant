namespace DomainLayer.Entites {
	public class UserCoupon : BaseEntity {
		public Guid CouponId { get; set; }
		public string UserId { get; set; }
		public virtual Coupon Coupon { get; set; }
		public virtual ApplicationUser User { get; set; }
	}
}
