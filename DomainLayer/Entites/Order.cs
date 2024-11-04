namespace DomainLayer.Entites {
	public class Order : BaseEntity {
		/// <summary>
		/// Loại đơn hàng (1 - phục vụ tại bàn, 2 - mang về, 3 - giao hàng)
		/// </summary>
		public int OrderType { get; set; }
		//public Guid BranchId { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public string CustomerId { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string? ShippingAddress { get; set; }
		/// <summary>
		/// Phí giao hàng
		/// </summary>
		public double DeliveryAmount { get; set; }
		/// <summary>
		/// Tiền khách đặt trước
		/// </summary>
		public double DepositAmount { get; set; }
		public double TotalAmount { get; set; }

		public ApplicationUser Customer { get; set; }
		public virtual IEnumerable<OrderDetail> Details { get; set; }
		public Invoice? Invoice { get; set; }
		//public Branch Branch { get; set; }
	}
}
