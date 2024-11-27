using ApplicationLayer.DTOs.Requests.Account;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Account;

namespace ApplicationLayer.Interfaces {
	public interface IAuthService {
		Task<AuthResponseDto> Login(LoginRequestDto request);
		Task<AuthResponseDto> Register(RegistrationRequestDto request);
		Task<AuthResponseDto> RefreshToken(TokenRequestDto request);
		Task<GeneralResponse> ChangePassword(ChangePasswordRequest request);
		Task<GeneralResponse> ResetPassword(ResetPasswordRequest request);
		Task<GeneralResponse> ForgotPassword(ForgotPasswordRequest request);
		Task<GeneralResponse> VerifyEmail(VerifyEmailRequest request);
	}
}
