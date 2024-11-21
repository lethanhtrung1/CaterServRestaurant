namespace ApplicationLayer.DTOs.Requests.User {
	public class CreateUserRequest {
		public string Email { get; set; }
		public string Password { get; set; }
		public string Role { get; set; }
	}
}
