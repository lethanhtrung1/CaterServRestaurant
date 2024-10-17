using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.DTOs.Responses.Account;
using ApplicationLayer.Interfaces;

namespace InfrastructrureLayer.Authentication {
	public class AuthService : IAuthService {
		public Task<AuthResponseDto> Login(LoginRequestDto request) {
			throw new NotImplementedException();
		}

		public Task<AuthResponseDto> Register(RegistrationRequestDto request) {
			throw new NotImplementedException();
		}
	}
}
