namespace DomainLayer.Entites {
	public class Notification : BaseEntity {
		public string Title { get; set; }
		public string Description { get; set; }
		public string Content { get; set; }
		public DateTime SendTime { get; set; }
		public bool Status { get; set; }
		public bool IsDelete { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string? UserName { get; set; }
		public string? UserId { get; set; }
		public virtual ApplicationUser? User { get; set; }
	}
}
