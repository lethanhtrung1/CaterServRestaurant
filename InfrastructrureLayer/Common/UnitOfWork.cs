using DomainLayer.Common;
using DomainLayer.Repositories;
using InfrastructrureLayer.Data;
using InfrastructrureLayer.Repositories;

namespace InfrastructrureLayer.Common {
	public class UnitOfWork : IUnitOfWork {
		private readonly AppDbContext _dbContext;
		public IBookingRepository Booking { get; set; }
		public IBookingTableRepository BookingTable { get; set; }
		//public IBranchRepository Branch { get; set; }
		public ICategoryRepository Category { get; set; }
		public ICouponRepository Coupon { get; set; }
		public IInvoiceRepository Invoice { get; set; }
		public IInvoiceCouponRepository InvoiceCoupon { get; set; }
		public IInvoiceDetailRepository InvoiceDetail { get; set; }
		public IInvoicePaymentRepository InvoicePayment { get; set; }
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

		public UnitOfWork(AppDbContext dbContext) {
			_dbContext = dbContext;
			Booking = new BookingRepository(_dbContext);
			BookingTable = new BookingTableRepository(_dbContext);
			//Branch = new BranchRepository(_dbContext);
			Category = new CategoryRepository(_dbContext);
			Coupon = new CouponRepository(_dbContext);
			Invoice = new InvoiceRepository(_dbContext);
			InvoiceCoupon = new InvoiceCouponRepository(_dbContext);
			InvoiceDetail = new InvoiceDetailRepository(_dbContext);
			InvoicePayment = new InvoicePaymentRepository(_dbContext);
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
		}

		public async Task SaveChangeAsync() {
			await _dbContext.SaveChangesAsync();
		}
		
		public void Dispose() {
			_dbContext.Dispose();
		}
	}
}
