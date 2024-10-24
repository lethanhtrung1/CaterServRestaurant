namespace DomainLayer.Entites {
	public class ProductImage : BaseEntity {
		public Guid ProductId { get; set; }
		public string ImageUrl {  get; set; }
		public Product? Product { get; set; }
	}
}
