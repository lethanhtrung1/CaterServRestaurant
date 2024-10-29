using DomainLayer.Repositories;

namespace DomainLayer.Common {
	public interface IUnitOfWork : IDisposable {
		Task SaveChangeAsync();
		IBookingRepository Booking {  get; }
		IBookingTableRepository BookingTable { get; }
		IBranchRepository Branch { get; }
		ICategoryRepository Category { get; }
		ICouponRepository Coupon { get; }
		IInvoiceRepository Invoice { get; }
		IInvoiceCouponRepository InvoiceCoupon { get; }
		IInvoiceDetailRepository InvoiceDetail { get; }
		IInvoicePaymentRepository InvoicePayment { get; }
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
	}
}
