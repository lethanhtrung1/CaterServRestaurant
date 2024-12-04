namespace ApplicationLayer.DTOs.Responses.Order {
	public class OrderResponse {
		public Guid Id { get; set; }
		public Guid? BookingId { get; set; }
		public int OrderType { get; set; }
		public string? OrderTypeName { get; set; }
		public string? OrderStatus { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastUpdatedAt { get; set; }
		public DateTime ShippingDate { get; set; }
		public string? CustomerId { get; set; }
		public string? CustomerName { get; set; }
		public string? CustomerPhone { get; set; }
		public string? ShippingAddress { get; set; }
		public decimal DeliveryAmount { get; set; }
		public decimal DepositAmount { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal TotalAmount { get; set; }

		public List<OrderDetailResponse>? OrderDetails { get; set; }
	}
}
