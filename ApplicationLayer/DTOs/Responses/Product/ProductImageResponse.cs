namespace ApplicationLayer.DTOs.Responses.Product {
	public class ProductImageResponse {
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string? ImageUrl { get; set; }
	}
}
