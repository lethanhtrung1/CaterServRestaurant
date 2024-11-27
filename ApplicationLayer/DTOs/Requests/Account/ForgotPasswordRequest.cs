using System.ComponentModel.DataAnnotations;

namespace ApplicationLayer.DTOs.Requests.Account {
	public class ForgotPasswordRequest {
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
