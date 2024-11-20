namespace ApplicationLayer.DTOs.Requests.Order {
	public class CreateOrderRequest {
		public Guid MealId { get; set; }
		public Guid? TableId { get; set; }
		public int OrderType { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string? ShippingAddress { get; set; }
		//public decimal DeliveryAmount { get; set; }
		//public decimal DepositAmount { get; set; }
		//public decimal DiscountAmount { get; set; }
		//public decimal TotalAmount { get; set; }
	}
}
