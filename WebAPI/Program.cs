using ApplicationLayer.DependencyInjection;
using InfrastructrureLayer.DependencyInjection;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationService();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddServiceExtensions(builder.Configuration);

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
