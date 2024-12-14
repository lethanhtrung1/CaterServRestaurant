using ApplicationLayer.Middleware;
using InfrastructrureLayer.Data;

namespace WebAPI.Extensions {
	public static class ApplicationExtensions {
		public static IApplicationBuilder UseApplicationPolicy(this IApplicationBuilder app) {
			app.ConfigureCustomMiddleware();
			app.SeedDatabase();
			return app;
		}

		public static IApplicationBuilder ConfigureCustomMiddleware(this IApplicationBuilder app) {
			app.UseMiddleware<ExceptionMiddleware>();
			return app;
		}

		public static void SeedDatabase(this IApplicationBuilder app) {
			using (var scope = app.ApplicationServices.CreateScope()) {
				var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
				dbInitializer.Initialize();
			}
		}
	}
}
