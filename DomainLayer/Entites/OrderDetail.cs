namespace DomainLayer.Entites {
	public class OrderDetail : BaseEntity {
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }
		public Guid OrderId { get; set; }
		public string UnitName {  get; set; }
		public int Quantity { get; set; }
		public double Price { get; set; }
		public double TotalPrice { get; set; }

		public Order Order { get; set; }
		public Product Product { get; set; }
		//public InvoiceDetail InvoiceDetail { get; set; }
	}
}
