namespace ApplicationLayer.DTOs.Requests.Account {
	public class RevokeRefreshTokenRequestDto {
		public string UserId { get; set; }
		public string RefreshToken { get; set; }
	}
}
