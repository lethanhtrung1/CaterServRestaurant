﻿using ApplicationLayer.Common.Consumer;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using ApplicationLayer.Options;
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

			return services;
		}
	}
}
