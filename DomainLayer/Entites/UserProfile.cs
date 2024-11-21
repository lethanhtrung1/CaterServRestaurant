namespace DomainLayer.Entites {
	public class UserProfile : BaseEntity {
		public string UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName => FirstName + " " + LastName;
		public string Gender { get; set; }
		public DateTime Birthday { get; set; }
		public string PhoneNumber { get; set; }
		public string Address { get; set; }
		public string? Avatar { get; set; }
		//public int Point {  get; set; }
		public string? Bank { get; set; }
		public string? BankBranch { get; set; }
		public string? BankNumber { get; set; }

		public ApplicationUser User { get; set; }
	}
}
