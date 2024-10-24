using DomainLayer.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfrastructrureLayer.Data {
	public class AppDbContext : IdentityDbContext<ApplicationUser, UserRole, string> {
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		#region DbSet

		public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<RefreshToken>(entity => {
				entity.HasOne(d => d.User)
					.WithMany(p => p.RefreshTokens)
					.HasForeignKey(d => d.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});
		}
	}
}
