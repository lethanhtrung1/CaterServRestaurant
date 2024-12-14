using DomainLayer.Repositories;

namespace DomainLayer.Common {
	public interface IUnitOfWork : IDisposable {
		Task SaveChangeAsync();
		IBookingRepository Booking {  get; }
		IBookingTableRepository BookingTable { get; }
		ICategoryRepository Category { get; }
		ICouponRepository Coupon { get; }
		IMealRepository Meal { get; }
		IMealProductRepository MealProduct { get; }
		IMenuRepository Menu { get; }
		IOrderRepository Order { get; }
		IOrderDetailRepository OrderDetail { get; }
		IProductRepository Product { get; }
		IProductImageRepository ProductImage { get; }
		ITableRepository Table { get; }
		IUserProfileRepository UserProfile { get; }
		IRefreshTokenRepository RefreshToken { get; }
		IMerchantRepository Merchant { get; }
		IPaymentRepository Payment { get; }
		IPaymentDestinationRepository PaymentsDestination { get; }
		IPaymentSignatureRepository PaymentSignature { get; }
		IApplicationUserRepository ApplicationUser { get; }
		IReviewRepository Review { get; }
		INotificationRepository Notification { get; }
		IUserCouponRepository UserCoupon { get; }
	}
}
