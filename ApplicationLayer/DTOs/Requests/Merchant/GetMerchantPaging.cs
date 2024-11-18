using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Merchant {
	public class GetMerchantPaging : PagingRequest {
		public int IsActive { get; set; } = 0;
	}
}
