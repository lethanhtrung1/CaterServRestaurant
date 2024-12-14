using Microsoft.AspNetCore.Identity;

namespace DomainLayer.Entites {
	public class ApplicationUser : IdentityUser {
		public string? Name { get; set; }
		public virtual IEnumerable<RefreshToken>? RefreshTokens { get; set; }
		public virtual IEnumerable<Booking>? Bookings { get; set; }
		public UserProfile UserProfile { get; set; }
		public virtual Meal? Meal { get; set; }
		public virtual IEnumerable<Order>? Orders { get; set; }
		public virtual IEnumerable<Review>? Reviews { get; set; }
		//public virtual IEnumerable<Notification>? Notifications { get; set; }
		public virtual IEnumerable<UserCoupon>? UserCoupons { get; set; }
	}
}
