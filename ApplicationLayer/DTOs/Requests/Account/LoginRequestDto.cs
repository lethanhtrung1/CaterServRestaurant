using System.ComponentModel.DataAnnotations;

namespace ApplicationLayer.DTOs.Requests.Account {
	public class LoginRequestDto {
		[Required]
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; } = string.Empty;
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;
	}
}
