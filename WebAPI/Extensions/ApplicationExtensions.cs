using ApplicationLayer.Middleware;

namespace WebAPI.Extensions {
	public static class ApplicationExtensions {
		public static IApplicationBuilder ConfigureCustomMiddleware(this IApplicationBuilder app) {
			app.UseMiddleware<ExceptionMiddleware>();
			return app;
		}

		public static IApplicationBuilder UseApplicationPolicy(this IApplicationBuilder app) {
			app.ConfigureCustomMiddleware();
			//app.UseCors("allowSpecificOrigins");

			return app;
		}
	}
}
