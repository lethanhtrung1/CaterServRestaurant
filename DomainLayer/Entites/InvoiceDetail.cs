namespace DomainLayer.Entites {
	public class InvoiceDetail : BaseEntity {
		public Guid InvoiceId { get; set; }
		public Guid ProductId { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public string UnitName {  get; set; }
		public string? Description { get; set; }
		public decimal Amount { get; set; }
		public int SortOrder { get; set; }
		/// <summary>
		/// Tiền khuyến mại
		/// </summary>
		public decimal PromotionAmount { get; set; }
		public Guid OrderDetailId { get; set; }

		public Invoice? Invoice { get; set; }
		public Product? Product { get; set; }
		public OrderDetail? OrderDetail { get; set; }
	}
}
