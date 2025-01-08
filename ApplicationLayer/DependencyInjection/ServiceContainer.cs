using ApplicationLayer.Common.Consumer;
using ApplicationLayer.Interfaces;
using ApplicationLayer.MappingConfigs;
using ApplicationLayer.Services;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLayer.DependencyInjection {
	public static class ServiceContainer {
		public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration) {

			// Register automapper
			IMapper mapper = MappingExtension.RegisterMaps().CreateMapper();
			services.AddSingleton(mapper);
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			// DI
			services.AddScoped<ICurrentUserService, CurrentUserService>();
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IMenuService, MenuService>();
			services.AddScoped<ITableService, TableService>();
			services.AddScoped<ICouponService, CouponService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IProductImageService, ProductImageService>();
			services.AddScoped<IMealService, MealService>();
			services.AddScoped<IBookingService, BookingService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IOrderDetailService, OrderDetailService>();
			services.AddScoped<IMerchantService, MerchantService>();
			services.AddScoped<IPaymentDestinationService, PaymentDestinationService>();
			services.AddScoped<IPaymentSignatureService, PaymentSignatureService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IUserProfileService, UserProfileService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IReviewService, ReviewService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IDashboardService, DashboardService>();
			services.AddScoped<IUserCouponService, UserCouponService>();

			services.AddHangfire(config => config
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions {
					CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
					SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
					QueuePollInterval = TimeSpan.Zero,
					UseRecommendedIsolationLevel = true,
					DisableGlobalLocks = true
				})
			);
			services.AddHangfireServer();

			return services;
		}
	}
}
