namespace DomainLayer.Entites {
	public class RefreshToken : BaseEntity {
		public string UserId { get; set; }
		public string Token { get; set; }
		public string JwtId { get; set; }
		public DateTime AddedDate { get; set; }
		public DateTime ExpiryDate { get; set; }
		public DateTime? RevokedDate { get; set; }
		public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
		//public bool IsRevoked => RevokedDate != null;
		public bool IsActive => !IsExpired && RevokedDate == null;
	}
}
