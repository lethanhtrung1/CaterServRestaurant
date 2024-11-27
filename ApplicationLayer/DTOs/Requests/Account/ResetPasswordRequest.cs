using System.ComponentModel.DataAnnotations;

namespace ApplicationLayer.DTOs.Requests.Account {
	public class ResetPasswordRequest {
		[Required(ErrorMessage = "Password is required.")]
		public string? Password { get; set; }
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string? ConfirmnPassword { get; set; }

		public string? Email { get; set; }
		public string? Code { get; set; }
	}
}
