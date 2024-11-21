using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Requests.UserProfile {
	public class CreateOrUpdateUserProfileRequest {
		public string UserId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Gender { get; set; }
		public DateTime Birthday { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public IFormFile? File { get; set; }
	}
}
