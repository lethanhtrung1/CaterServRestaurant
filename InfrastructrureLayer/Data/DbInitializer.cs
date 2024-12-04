using ApplicationLayer.Common.Constants;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InfrastructrureLayer.Data {
	public class DbInitializer : IDbInitializer {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<UserRole> _roleManager;
		private readonly AppDbContext _dbContext;

		public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager, AppDbContext dbContext) {
			_dbContext = dbContext;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public void Initialize() {
			// migration if not applied
			try {
				if (_dbContext.Database.GetPendingMigrations().Count() > 0) {
					_dbContext.Database.Migrate();
				}
			} catch (Exception) {

				throw;
			}

			// Create role if the are not created
			if (!_roleManager.RoleExistsAsync(Role.CUSTOMER).GetAwaiter().GetResult()) {
				_roleManager.CreateAsync(new UserRole { Name = Role.CUSTOMER, Description = "Customer role" }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new UserRole { Name = Role.STAFF, Description = "Staff role" }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new UserRole { Name = Role.ADMIN, Description = "Admin role" }).GetAwaiter().GetResult();

				// Create admin account
				_userManager.CreateAsync(new ApplicationUser {
					UserName = "admin@gmail.com",
					Email = "admin@gmail.com",
					Name = "Admin",
					PhoneNumber = "0969905002",
					EmailConfirmed = true,
				}, "admin123aA@").GetAwaiter().GetResult();

				ApplicationUser user = _userManager.FindByEmailAsync("admin@gmail.com").GetAwaiter().GetResult()!;
				_userManager.AddToRoleAsync(user, Role.ADMIN).GetAwaiter().GetResult();
			}

			return;
		}
	}
}
