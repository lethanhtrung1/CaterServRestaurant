namespace ApplicationLayer.DTOs.Requests.Account {
	public class VerifyEmailRequest {
		public string Email { get; set; }
		public string Code { get; set; }
	}
}
