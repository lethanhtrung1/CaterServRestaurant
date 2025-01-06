namespace ApplicationLayer.DTOs.Requests.Order {
	public class CreateOrderDetailRequest {
		public Guid OrderId { get; set; }
		public List<ProductRequest> Products { get; set; }
	}
}
