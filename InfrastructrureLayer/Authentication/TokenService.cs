﻿using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.DTOs.Responses.Account;
using ApplicationLayer.Interfaces;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InfrastructrureLayer.Authentication {
	public class TokenService : ITokenService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;
		private readonly IConfigurationSection _jwtOptions;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<ApplicationUser> _userManager;

		public TokenService(IUnitOfWork unitOfWork, IConfiguration configuration,
			IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) {
			_unitOfWork = unitOfWork;
			_configuration = configuration;
			_jwtOptions = _configuration.GetSection("AuthJwt");
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
		}

		public async Task<TokenResponseDto> GenerateToken(ApplicationUser user, bool populateExp) {
			var jwtTokenHandler = new JwtSecurityTokenHandler();

			var key = Encoding.UTF8.GetBytes(_jwtOptions["Key"]!);

			var tokenDescriptor = new SecurityTokenDescriptor() {
				Audience = _jwtOptions["Audience"],
				Issuer = _jwtOptions["Issuer"],
				Subject = new ClaimsIdentity(new[] {
					new Claim("Id", user.Id),
					new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
					new Claim(JwtRegisteredClaimNames.Email, user.Email!),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
					new Claim(ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).FirstOrDefault()!.ToString())
				}),
				Expires = DateTime.Now.Add(TimeSpan.Parse(_jwtOptions["ExpiryTimeFrame"]!)),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};

			var token = jwtTokenHandler.CreateToken(tokenDescriptor);

			if (populateExp) {
				var newRefreshToken = GenerateRefreshToken();
				var newRefreshTokenToDb = new RefreshToken() {
					JwtId = token.Id,
					Token = newRefreshToken,
					AddedDate = DateTime.Now,
					ExpiryDate = DateTime.Now.AddDays(7),
					IsUsed = false,
					IsRevoked = false,
					UserId = user.Id,
				};
				await _unitOfWork.RefreshToken.AddAsync(newRefreshTokenToDb);
				await _unitOfWork.SaveChangeAsync();
			}

			var accessToken = jwtTokenHandler.WriteToken(token);
			var refreshToken = await _unitOfWork.RefreshToken
				.GetAsync(x => x.UserId == user.Id && x.IsUsed == false && x.IsRevoked == false);
			return new TokenResponseDto(accessToken, refreshToken.Token);
		}

		private string GenerateRefreshToken() {
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create()) {
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}

		public async Task<TokenResponseDto> RefreshToken(TokenRequestDto token) {
			try {
				await _unitOfWork.RefreshToken.BeginTransactionAsync();

				var principal = GetPrincipalFromExpiredToken(token.AccessToken);

				var storedToken = await _unitOfWork.RefreshToken
					.GetAsync(x => x.Token == token.RefreshToken && x.IsUsed == false && x.IsRevoked == false);

				if (storedToken is null || storedToken.ExpiryDate < DateTime.Now) {
					throw new SecurityTokenException("Invalid Token");
				}

				var jti = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
				if (storedToken.JwtId != jti) {
					throw new SecurityTokenException("Invalid token");
				}

				storedToken.IsUsed = true;
				await _unitOfWork.RefreshToken.UpdateAsync(storedToken);
				await _unitOfWork.SaveChangeAsync();

				await _unitOfWork.RefreshToken.EndTransactionAsync();

				var userId = principal.Claims.FirstOrDefault(x => x.Type == "Id")!.Value;
				var user = await _userManager.FindByIdAsync(userId);

				return await GenerateToken(user!, populateExp: false);
			} catch (Exception) {
				await _unitOfWork.RefreshToken.RollBackTransactionAsync();
				throw;
			}
		}

		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token) {
			var tokenValidationParameters = new TokenValidationParameters {
				ValidateIssuerSigningKey = true,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = false, // don't care about the token's expiration date
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions["Key"]!)),
				ValidIssuer = _jwtOptions["Issuer"],
				ValidAudience = _jwtOptions["Audience"]
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken securityToken;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}

		public async Task RevokeRefreshToken(Guid userId, string token) {
			var refreshToken = await _unitOfWork.RefreshToken.GetAsync(x => x.Token == token);
			if (refreshToken == null || refreshToken.UserId != userId.ToString()) {
				throw new SecurityTokenException();
			}
			refreshToken.IsRevoked = true;
			await _unitOfWork.RefreshToken.UpdateAsync(refreshToken);
			await _unitOfWork.SaveChangeAsync();
		}

		public void SetTokensInsideCookie(TokenRequestDto token) {
			var context = _httpContextAccessor.HttpContext;
			if (context != null) {
				context.Response.Cookies.Append("accessToken", token.AccessToken, new CookieOptions {
					Expires = DateTimeOffset.UtcNow.AddDays(5),
					HttpOnly = true,
					IsEssential = true,
					Secure = true,
					SameSite = SameSiteMode.None,
				});
				context.Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions {
					Expires = DateTimeOffset.UtcNow.AddDays(7),
					HttpOnly = true,
					IsEssential = true,
					Secure = true,
					SameSite = SameSiteMode.None,
				});
			}
		}
	}
}
