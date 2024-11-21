namespace ApplicationLayer.DTOs.Responses.UserProfile {
	public class UserProfileResponse {
		public string UserId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Gender { get; set; }
		public DateTime Birthday { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public string? Avatar { get; set; }
	}
}
