namespace ApplicationLayer.DTOs.Responses.UserProfile {
	public class StaffProfileResponse : UserProfileResponse {
		public string? Bank { get; set; }
		public string? BankBranch { get; set; }
		public string? BankNumber { get; set; }
	}
}
