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
		public virtual DbSet<Branch> Branches { get; set; }
		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Coupon> Coupons { get; set; }
		public virtual DbSet<Invoice> Invoices { get; set; }
		public virtual DbSet<InvoiceCoupon> InvoiceCoupons { get; set; }
		public virtual DbSet<InvoiceDetail> InvoicesDetails { get; set; }
		public virtual DbSet<InvoicePayment> InvoicesPayments { get; set; }
		public virtual DbSet<Menu> Menus { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<OrderDetail> OrderDetails { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<ProductImage> ProductImages { get; set; }
		//public virtual DbSet<Restaurant> Restaurants { get; set; }
		public virtual DbSet<Table> Tables { get; set; }
		public virtual DbSet<UserProfile> UserProfiles { get; set; }
		public virtual DbSet<Meal> Meals { get; set; }
		public virtual DbSet<MealProduct> MealsProducts { get; set; }

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
			});

			modelBuilder.Entity<Table>(entity => {
				entity.HasMany(d => d.BookingTables)
					.WithOne(p => p.Table)
					.OnDelete(DeleteBehavior.Cascade);
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
		}
	}
}
