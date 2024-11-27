namespace DomainLayer.Entites {
	public class ProductImage : BaseEntity {
		public Guid ProductId { get; set; }
		public string ImageUrl {  get; set; }
		public string PublicId { get; set; } // public Id image from cloudinary
		public Product? Product { get; set; }
	}
}
