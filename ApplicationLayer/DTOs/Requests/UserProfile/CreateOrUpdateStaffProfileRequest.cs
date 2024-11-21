using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Requests.UserProfile {
	public class CreateOrUpdateStaffProfileRequest : CreateOrUpdateUserProfileRequest {
		public string? Bank {  get; set; }
		public string? BankBranch { get; set; }
		public string? BankNumber { get; set; }
	}
}
