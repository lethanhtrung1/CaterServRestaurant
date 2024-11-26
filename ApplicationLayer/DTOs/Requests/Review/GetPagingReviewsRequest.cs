using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Review {
	public class GetPagingReviewsRequest : PagingRequest {
		public Guid ProductId { get; set; }
		public int? Rating { get; set; }
		public string? SortBy { get; set; }
	}
}
