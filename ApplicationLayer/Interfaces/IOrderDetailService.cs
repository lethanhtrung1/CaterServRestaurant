using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Order;

namespace ApplicationLayer.Interfaces {
	public interface IOrderDetailService {
		Task<ApiResponse<List<OrderDetailResponse>>> GetOrderDetail(Guid orderId);
		Task<ApiResponse<PagedList<OrderDetailResponse>>> GetOrderDetailsPaging(PagingRequest request);
	}
}
