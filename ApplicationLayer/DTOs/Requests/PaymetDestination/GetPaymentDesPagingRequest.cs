using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.PaymetDestination {
	public class GetPaymentDesPagingRequest : PagingRequest {
		public int IsActive { get; set; } = 0;
	}
}
