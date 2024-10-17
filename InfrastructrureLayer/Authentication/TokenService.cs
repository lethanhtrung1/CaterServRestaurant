using ApplicationLayer.DTOs.Responses.Account;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructrureLayer.Authentication {
	public class TokenService : ITokenService {
		public Task<TokenResponseDto> GenerateAsscessToken(ApplicationUser user, bool populateExp) {
			throw new NotImplementedException();
		}

		public Task<TokenResponseDto> RefreshToken(TokenResponseDto token) {
			throw new NotImplementedException();
		}

		public Task RevokeRefreshToken(Guid userId, string token) {
			throw new NotImplementedException();
		}

		public void SetTokensInsideCookie(TokenResponseDto token) {
			throw new NotImplementedException();
		}
	}
}
