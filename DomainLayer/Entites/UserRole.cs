using Microsoft.AspNetCore.Identity;

namespace DomainLayer.Entites {
	public class UserRole : IdentityRole {
		public string Description { get; set; }
	}
}
