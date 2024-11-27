namespace ApplicationLayer.DTOs.Responses.User {
	public class UserResponse {
		public string Id { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public bool IsBanned { get; set; }
	}
}
