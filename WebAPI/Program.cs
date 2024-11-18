using ApplicationLayer.DependencyInjection;
using InfrastructrureLayer.Data;
using InfrastructrureLayer.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationService();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddServiceExtensions(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options => {
	options.AddPolicy(
		name: "allowSpecificOrigins",
		policy => {
			policy.WithOrigins("*")
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials();
		}
	);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseApplicationPolicy();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions {
	FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
	RequestPath = new PathString("/Resources")
});

app.UseAuthentication();

app.UseAuthorization();

SeedDatabase();

app.MapControllers();

app.Run();

void SeedDatabase() {
	using (var scope = app.Services.CreateScope()) {
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}
}