namespace DomainLayer.Entites {
	public class Product : BaseEntity {
		public string Code { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public double Price { get; set; }
		public double SellingPrice { get; set; }
		public string UnitName {  get; set; }
		public bool Inactive { get; set; }
		public Guid CategoryId { get; set; }
		public Guid MenuId { get; set; }

		public Category Category { get; set; }
		public Menu Menu { get; set; }
		public virtual IEnumerable<OrderDetail>? OrderDetails { get; set; }
		public virtual IEnumerable<InvoiceDetail>? InvoiceDetails { get; set; }
		public virtual IEnumerable<ProductImage> ProductImages { get; set; }
	}
}
