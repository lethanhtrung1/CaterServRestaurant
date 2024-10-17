using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.DTOs.Responses.Account;
using ApplicationLayer.Interfaces;
using DomainLayer.Common;
using DomainLayer.Entites;
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
		private readonly UserManager<ApplicationUser> _userManager;

		public TokenService(IUnitOfWork unitOfWork, IConfiguration configuration, UserManager<ApplicationUser> userManager) {
			_unitOfWork = unitOfWork;
			_configuration = configuration;
			_jwtOptions = _configuration.GetSection("AuthJwt");
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

		public Task<TokenResponseDto> RefreshToken(TokenRequestDto token) {
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}
	}
}
