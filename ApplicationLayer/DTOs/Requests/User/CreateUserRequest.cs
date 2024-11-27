namespace ApplicationLayer.DTOs.Requests.User {
	public class CreateUserRequest {
		public string Email { get; set; }
		public string Password { get; set; }
		public string RoleId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Gender { get; set; }
		public DateTime Birthday { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public string? Bank { get; set; }
		public string? BankBranch { get; set; }
		public string? BankNumber { get; set; }
	}
}
