namespace ApplicationLayer.DTOs.Requests.Order {
	public class UpdateOrderRequest {
		public Guid Id { get; set; }
		public Guid? TableId { get; set; }
		public Guid MealId { get; set; }
		public int OrderType { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime ShippingDate { get; set; }
		public string? ShippingAddress { get; set; }
		public decimal DeliveryAmount { get; set; }
		public decimal DepositAmount { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal TotalAmount { get; set; }
	}
}
