namespace ApplicationLayer.DTOs.Responses.Product {
	public class ProductCategoryDto {
		public Guid Id { get; set; }
		public string? Code { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
	}
}
