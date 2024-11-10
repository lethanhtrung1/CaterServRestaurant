using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Product {
	public class FilterProductRequest : PagingRequest {
		public string? SearchText { get; set; }
		public bool InActive { get; set; }
		public decimal PriceFrom { get; set; }
		public decimal PriceTo { get; set; }
		public Guid CategoryId { get; set; }
		public Guid MenuId { get; set; }
	}
}
