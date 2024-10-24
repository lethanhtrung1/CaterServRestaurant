namespace DomainLayer.Entites {
	public class Notification : BaseEntity {
		public string Title { get; set; }
		public string Description { get; set; }
		public string Content { get; set; }
		public DateTime SeedTime { get; set; }
		public bool Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }

	}
}
