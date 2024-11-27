namespace DomainLayer.Entites {
	public class Menu : BaseEntity {
		public string MenuName { get; set; }
		public string? Description { get; set; }
		public bool Inactive { get; set; }
		public int SortOrder { get; set; }
		public string? ImageUrl { get; set; }
		public string? MenuPublicId { get; set; }

		public virtual IEnumerable<Product> Products { get; set; }
	}
}
