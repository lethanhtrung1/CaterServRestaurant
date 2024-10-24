namespace DomainLayer.Entites {
	public class Cart : BaseEntity {
		public string CustomerId { get; set; }
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
		public DateTime CreatedDate { get; set; }

		public ApplicationUser? Customer { get; set; }
		public Product? Product { get; set; }
	}
}
