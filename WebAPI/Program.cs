using ApplicationLayer.DependencyInjection;
using ApplicationLayer.Hubs;
using ApplicationLayer.Options;
using Hangfire;
using InfrastructrureLayer.DependencyInjection;
using WebAPI.Extensions;

public class Program {
	private static void Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddApplicationService(builder.Configuration);
		builder.Services.AddInfrastructureService(builder.Configuration);
		builder.Services.AddServiceExtensions(builder.Configuration);

		builder.Services.Configure<VnpayOptions>(builder.Configuration.GetSection(VnpayOptions.ConfigName));
		builder.Services.Configure<MomoOptions>(builder.Configuration.GetSection(MomoOptions.ConfigName));
		builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection(CloudinaryOptions.ConfigName));

		builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		//builder.Services.AddDistributedMemoryCache();
		//builder.Services.AddSession();
		builder.Services.AddSignalR();
		builder.Services.AddHttpContextAccessor();

		builder.Services.AddCors(options => {
			options.AddPolicy(
				name: "allowSpecificOrigins",
				policy => {
					policy.WithOrigins(
						"https://localhost:5173", "http://localhost:5173",
						"https://localhost:5174", "http://localhost:5174"
					).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
				}
			);
		});

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		// Use custom middleware
		app.UseApplicationPolicy();

		app.UseHttpsRedirection();

		app.UseStaticFiles();

		app.UseCors("allowSpecificOrigins");

		app.UseAuthentication();

		app.UseAuthorization();

		app.MapHub<NotificationHub>("/notificationHub");

		app.UseHangfireDashboard();
		app.UseHangfireDashboard("/hangfire");

		app.MapControllers();

		app.Run();
	}
}