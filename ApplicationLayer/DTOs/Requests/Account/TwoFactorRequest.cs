using System.ComponentModel.DataAnnotations;

namespace ApplicationLayer.DTOs.Requests.Account {
	public class TwoFactorRequest {
		[Required]
		public string? Email { get; set; }
		//[Required]
		//public string? Provider { get; set; }
		[Required]
		public string? Token { get; set; }
	}
}
