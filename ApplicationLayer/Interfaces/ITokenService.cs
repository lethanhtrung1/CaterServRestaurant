using ApplicationLayer.DTOs.Responses.Account;
using DomainLayer.Entites;

namespace ApplicationLayer.Interfaces {
	public interface ITokenService {
		Task<TokenResponseDto> GenerateAsscessToken(ApplicationUser user, bool populateExp);
		Task<TokenResponseDto> RefreshToken(TokenResponseDto token);
		Task RevokeRefreshToken(Guid userId, string token);
		void SetTokensInsideCookie(TokenResponseDto token);
	}
}
