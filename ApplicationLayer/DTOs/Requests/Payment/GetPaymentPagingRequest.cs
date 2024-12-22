using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Payment {
	public class GetPaymentPagingRequest : PagingRequest {
		public string? From { get; set; }
		public string? To { get; set; }
	}
}
