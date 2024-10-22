using Microsoft.AspNetCore.Identity;

namespace DomainLayer.Entites {
	public class ApplicationUser : IdentityUser {
		public string? Name { get; set; }
		public List<RefreshToken> RefreshTokens { get; set; }
	}
}
