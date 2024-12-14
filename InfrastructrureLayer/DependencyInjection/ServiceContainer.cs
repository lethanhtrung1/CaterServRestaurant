using ApplicationLayer.Common.Consumer;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using ApplicationLayer.Options;
using DomainLayer.Caching;
using DomainLayer.Common;
using DomainLayer.Entites;
using InfrastructrureLayer.Authentication;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;
using InfrastructrureLayer.Logging;
using InfrastructrureLayer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace InfrastructrureLayer.DependencyInjection {
	public static class ServiceContainer {
		public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration) {
			// Config connection string
			services.AddDbContext<AppDbContext>(options => {
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddIdentity<ApplicationUser, UserRole>(options => {
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 8;
				options.Password.RequiredUniqueChars = 2;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = true;

				options.User.RequireUniqueEmail = true;

				// User lockout
				options.Lockout.AllowedForNewUsers = true;
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;

				// Reset password
				options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;

				// Confirmation Email
				//options.SignIn.RequireConfirmedEmail = true;
				options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;

				// 2FA
				options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultEmailProvider;
			})
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();

			// Configure logging
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.Debug()
				.WriteTo.Console()
				.WriteTo.File(
					path: "Logs/log.txt",
					restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
					rollingInterval: RollingInterval.Day
				)
				.CreateLogger();

			services.AddScoped<ILogException, LogException>();
			services.AddScoped<IEmailService, EmailService>();

			var emailOptions = configuration.GetSection("EmailOptions").Get<EmailOptions>();
			if (emailOptions == null) {
				throw new InvalidOperationException("Email Options are not configured correctly.");
			}
			services.AddSingleton(emailOptions);

			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<IDbInitializer, DbInitializer>();

			services.AddSingleton<ConnectionHelper>();
			services.AddScoped<ICacheService, CacheService>();

			return services;
		}
	}
}
