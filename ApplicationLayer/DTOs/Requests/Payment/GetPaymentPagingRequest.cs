using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Payment {
	public class GetPaymentPagingRequest : PagingRequest {
		public DateTime? From { get; set; }
		public DateTime? To { get; set; }
	}
}
