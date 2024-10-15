namespace DomainLayer.Entites {
	public class RefreshToken : BaseEntity {
		public string UserId { get; private set; }
		public string Token { get; private set; }
		public string JwtId { get; set; }
		public bool IsUsed { get; set; }
		public bool IsRevoked { get; set; }
		public DateTime AddedDate { get; set; }
		public DateTime ExpiryDate { get; set; }
	}
}
