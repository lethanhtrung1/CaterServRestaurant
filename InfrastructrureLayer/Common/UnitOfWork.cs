using DomainLayer.Common;
using DomainLayer.Repositories;
using InfrastructrureLayer.Data;
using InfrastructrureLayer.Repositories;

namespace InfrastructrureLayer.Common {
	public class UnitOfWork : IUnitOfWork {
		private readonly AppDbContext _dbContext;
		public IBookingRepository Booking { get; set; }
		public IBookingTableRepository BookingTable { get; set; }
		public ICategoryRepository Category { get; set; }
		public ICouponRepository Coupon { get; set; }
		public IMealRepository Meal { get; set; }
		public IMealProductRepository MealProduct { get; set; }
		public IMenuRepository Menu { get; set; }
		public IOrderRepository Order { get; set; }
		public IOrderDetailRepository OrderDetail { get; set; }
		public IProductRepository Product { get; set; }
		public IProductImageRepository ProductImage { get; set; }
		public ITableRepository Table { get; set; }
		public IUserProfileRepository UserProfile { get; set; }
		public IRefreshTokenRepository RefreshToken {  get; set; }
		public IMerchantRepository Merchant {  get; set; }
		public IPaymentRepository Payment { get; set; }
		public IPaymentDestinationRepository PaymentsDestination { get; set; }
		public IPaymentSignatureRepository PaymentSignature { get; set; }
		public IApplicationUserRepository ApplicationUser { get; set; }
		public IReviewRepository Review { get; set; }
		public INotificationRepository Notification { get; set; }
		public IUserCouponRepository UserCoupon { get; set; }

		public UnitOfWork(AppDbContext dbContext) {
			_dbContext = dbContext;
			Booking = new BookingRepository(_dbContext);
			BookingTable = new BookingTableRepository(_dbContext);
			Category = new CategoryRepository(_dbContext);
			Coupon = new CouponRepository(_dbContext);
			Meal = new MealRepository(_dbContext);
			MealProduct = new MealProductRepository(_dbContext);
			Menu = new MenuRepository(_dbContext);
			Order = new OrderRepository(_dbContext);
			OrderDetail = new OrderDetailRepository(_dbContext);
			Product = new ProductRepository(_dbContext);
			ProductImage = new ProductImageRepository(_dbContext);
			Table = new TableRepository(_dbContext);
			UserProfile = new UserProfileRepository(_dbContext);
			RefreshToken = new RefreshTokenRepository(_dbContext);
			Merchant = new MerchantRepository(_dbContext);
			Payment = new PaymentRepository(_dbContext);
			PaymentsDestination = new PaymentDestinationRepository(_dbContext);
			PaymentSignature = new PaymentSignatureRepository(_dbContext);
			ApplicationUser = new ApplicationUserRepository(_dbContext);
			Review = new ReviewRepository(_dbContext);
			Notification = new NotificationRepository(_dbContext);
			UserCoupon = new UserCouponRepository(_dbContext);
		}

		public async Task SaveChangeAsync() {
			await _dbContext.SaveChangesAsync();
		}
		
		public void Dispose() {
			_dbContext.Dispose();
		}
	}
}
