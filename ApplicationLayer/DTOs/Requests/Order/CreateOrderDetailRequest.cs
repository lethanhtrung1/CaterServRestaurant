namespace ApplicationLayer.DTOs.Requests.Order {
	public class CreateOrderDetailRequest {
		public Guid OrderId { get; set; }
		public List<Guid> ProductId { get; set; }
		public int Quantity { get; set; }
	}
}
