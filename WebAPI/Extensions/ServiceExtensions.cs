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

			if (jwtOptions == null) {
				throw new InvalidOperationException("JWT options are not configured correctly.");
			}

			services.AddSingleton(jwtOptions);

			// Add Jwt authentication scheme
			services.AddAuthentication(options => {
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options => {
				options.SaveToken = true;

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
			});

			return services;
		}

		private static IServiceCollection AddSwaggerUI(this IServiceCollection services) {
			services.AddSwaggerGen(options => {
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
