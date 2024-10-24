namespace DomainLayer.Entites {
	public class Category : BaseEntity {

		public string Code { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string? Description { get; set; }
		public bool Inactive { get; set; }
		public virtual IEnumerable<Product> Products { get; set; }
	}
}
