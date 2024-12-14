using ApplicationLayer.Common.Constants;
using ApplicationLayer.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace WebAPI.Extensions {
	public static class ServiceExtensions {
		public static IServiceCollection AddServiceExtensions(this IServiceCollection services, IConfiguration configuration) {
			services.AuthenticationService(configuration);
			services.AddSwaggerUI();

			return services;
		}

		private static IServiceCollection AuthenticationService(this IServiceCollection services, IConfiguration configuration) {
			var jwtOptions = configuration.GetSection(ApplicationConfig.JWT).Get<JwtOptions>();
			var googleOptions = configuration.GetSection(ApplicationConfig.GOOGLE).Get<GoogleOptions>();

			if (jwtOptions == null) {
				throw new InvalidOperationException("JWT options are not configured correctly");
			}

			if (googleOptions == null) {
				throw new InvalidOperationException("Google options are not configured correctly");
			}

			services.AddSingleton(jwtOptions);
			services.AddSingleton(googleOptions);

			// Add Jwt authentication scheme
			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options => {
				options.SaveToken = false;

				options.TokenValidationParameters = new TokenValidationParameters {
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidAudience = jwtOptions.Audience,
					ValidIssuer = jwtOptions.Issuer,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
					ClockSkew = TimeSpan.Zero,
				};
				//// Use HttpOnly Cookie
				options.Events = new JwtBearerEvents {
					OnMessageReceived = context => {
						context.Request.Cookies.TryGetValue("accessToken", out var accessToken);
						if (!string.IsNullOrEmpty(accessToken)) {
							context.Token = accessToken;
						}
						return Task.CompletedTask;
					}
				};
			}).AddGoogle(options => {
				options.ClientId = googleOptions.ClientId;
				options.ClientSecret = googleOptions.ClientSecret;
				//options.SaveTokens = true;
			});

			return services;
		}

		private static IServiceCollection AddSwaggerUI(this IServiceCollection services) {
			services.AddSwaggerGen(options => {
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant Management", Version = "v1" });
				options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme {
					Name = "Authorization",
					Description = "Enter the Bear Authorization string as following: `Bearer Generate-JWT-Token`",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement {
					{
						new OpenApiSecurityScheme {
							Reference = new OpenApiReference {
								Type = ReferenceType.SecurityScheme,
								Id = JwtBearerDefaults.AuthenticationScheme
							}
						},
						new List<string>()
					}
				});
			});

			return services;
		}
	}
}
