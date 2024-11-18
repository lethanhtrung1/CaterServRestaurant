using DomainLayer.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfrastructrureLayer.Data {
	public class AppDbContext : IdentityDbContext<ApplicationUser, UserRole, string> {
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		#region DbSet

		public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
		public virtual DbSet<Booking> Bookings { get; set; }
		public virtual DbSet<BookingTable> BookingsTables { get; set; }
		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Coupon> Coupons { get; set; }
		public virtual DbSet<Menu> Menus { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<OrderDetail> OrderDetails { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<ProductImage> ProductImages { get; set; }
		public virtual DbSet<Table> Tables { get; set; }
		public virtual DbSet<UserProfile> UserProfiles { get; set; }
		public virtual DbSet<Meal> Meals { get; set; }
		public virtual DbSet<MealProduct> MealsProducts { get; set; }
		public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
		//public virtual DbSet<Payment> Payments { get; set; }
		//public virtual DbSet<PaymentDestination> PaymentDestinations { get; set; }
		//public virtual DbSet<PaymentSignature> PaymentSignatures { get; set; }
		//public virtual DbSet<Merchant> Merchants { get; set; }
		//public virtual DbSet<UserCoupon> UserCoupons { get; set; }

		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<RefreshToken>(entity => {
				entity.HasOne(d => d.User)
					.WithMany(p => p.RefreshTokens)
					.HasForeignKey(d => d.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<BookingTable>(entity => {
				entity.HasOne(d => d.Booking)
					.WithMany(p => p.BookingTables)
					.HasForeignKey(d => d.BookingId)
					.OnDelete(DeleteBehavior.NoAction);

				entity.HasOne(d => d.Table)
					.WithMany(p => p.BookingTables)
					.HasForeignKey(d => d.TableId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			modelBuilder.Entity<Booking>(entity => {
				entity.HasMany(p => p.BookingTables)
					.WithOne(d => d.Booking)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(p => p.Customer)
					.WithMany(d => d.Bookings)
					.HasForeignKey(p => p.CustomerId)
					.IsRequired(false);
			});

			modelBuilder.Entity<Table>(entity => {
				entity.HasMany(d => d.BookingTables)
					.WithOne(p => p.Table)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Category>(entity => {
				entity.HasMany(d => d.Products)
					.WithOne(p => p.Category);
			});

			modelBuilder.Entity<Menu>(entity => {
				entity.HasMany(d => d.Products)
					.WithOne(p => p.Menu);
			});

			modelBuilder.Entity<Product>(entity => {
				entity.HasOne(d => d.Category)
					.WithMany(p => p.Products)
					.HasForeignKey(d => d.CategoryId)
					.OnDelete(DeleteBehavior.NoAction);

				entity.HasOne(d => d.Menu)
					.WithMany(p => p.Products)
					.HasForeignKey(d => d.MenuId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			modelBuilder.Entity<ProductImage>(entity => {
				entity.HasOne(d => d.Product)
					.WithMany(p => p.ProductImages)
					.HasForeignKey(d => d.ProductId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Order>(entity => {
				entity.HasOne(d => d.Booking)
					.WithMany(p => p.Orders)
					.HasForeignKey(d => d.BookingId)
					.IsRequired(false)
					.OnDelete(DeleteBehavior.NoAction);

				entity.HasOne(d => d.Customer)
					.WithMany(p => p.Orders)
					.HasForeignKey(d => d.CustomerId)
					.IsRequired(false);
			});

			modelBuilder.Entity<OrderDetail>(entity => {
				entity.HasOne(d => d.Order)
					.WithMany(p => p.OrderDetails)
					.HasForeignKey(d => d.OrderId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Meal>(entity => {
				entity.HasOne(d => d.Customer)
					.WithMany(p => p.Meals)
					.HasForeignKey(d => d.CustomerId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<MealProduct>(entity => {
				entity.HasOne(d => d.Meal)
					.WithMany(p => p.MealProducts)
					.HasForeignKey(d => d.MealId);

				entity.HasOne(d => d.Product)
					.WithMany(p => p.MealProducts)
					.HasForeignKey(d => d.ProductId);
			});

			modelBuilder.Entity<UserProfile>(entity => {
				entity.HasOne(d => d.User)
					.WithOne(p => p.UserProfile)
					.HasForeignKey<UserProfile>(d => d.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
