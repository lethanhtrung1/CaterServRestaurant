using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.DTOs.Responses.Account;
using DomainLayer.Entites;

namespace ApplicationLayer.Interfaces {
	public interface ITokenService {
		Task<TokenResponseDto> GenerateToken(ApplicationUser user, bool populateExp);
		Task<TokenResponseDto> RefreshToken(TokenRequestDto token);
		Task RevokeRefreshToken(Guid userId, string token);
		void SetTokensInsideCookie(TokenRequestDto token);
	}
}
