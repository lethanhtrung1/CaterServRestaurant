using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Order {
	public class GetOrdersPagingRequest : PagingRequest {
		public string? OrderStatus { get; set; }
		public int SortBy { get; set; } = 0;
	}
}
