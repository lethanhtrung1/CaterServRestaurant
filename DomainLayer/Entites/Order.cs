namespace DomainLayer.Entites {
	public class Order : BaseEntity {
		public Guid? BookingId { get; set; }
		/// <summary>
		/// Loại đơn hàng (1 - phục vụ tại bàn, 2 - mang về, 3 - giao hàng)
		/// </summary>
		public int OrderType { get; set; }
		public string? OrderStatus { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? ShippingDate { get; set; }
		public string? CustomerId { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string? ShippingAddress { get; set; }
		/// <summary>
		/// Phí giao hàng
		/// </summary>
		public decimal DeliveryAmount { get; set; }
		/// <summary>
		/// Tiền khách đặt trước
		/// </summary>
		public decimal DepositAmount { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal TotalAmount { get; set; }

		public ApplicationUser? Customer { get; set; }
		public virtual IEnumerable<OrderDetail> OrderDetails { get; set; }
		public Booking? Booking { get; set; }
	}
}
