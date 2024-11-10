namespace ApplicationLayer.DTOs.Responses.Product {
	public class ProductResponse {
		public string? Code { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal Price { get; set; }
		public decimal SellingPrice { get; set; }
		public string? UnitName { get; set; }
		public string? Thumbnail { get; set; }
		public bool Inactive { get; set; }
		
		public ProductMenuDto? MenuDto { get; set; }
		public ProductCategoryDto? CategoryDto { get; set; }
		public List<ProductImageDto>? ProductImagesDto { get; set; }
	}
}
