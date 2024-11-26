namespace ApplicationLayer.DTOs.Requests.Order {
	public class CreateOrderDetailRequest {
		public Guid OrderId { get; set; }
		public Guid ProductId { get; set; }
		//public string? ProductName { get; set; }
		//public string? UnitName { get; set; }
		//public decimal Price { get; set; }
		public int Quantity { get; set; }
		//public decimal TotalPrice { get; set; }
	}
}
