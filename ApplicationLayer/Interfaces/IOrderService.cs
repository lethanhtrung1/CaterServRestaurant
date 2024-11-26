using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Order;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Order;

namespace ApplicationLayer.Interfaces {
	public interface IOrderService {
		Task<ApiResponse<OrderResponse>> CreateOrder(CreateOrderRequest request);
		//Task<ApiResponse<OrderResponse>> UpdateOrder(UpdateOrderRequest request);
		Task<ApiResponse<OrderResponse>> UpdateOrderStatus(UpdateOrderStatusRequest request);
		Task<ApiResponse<OrderResponse>> GetOrderById(Guid orderId);
		Task<ApiResponse<OrderResponse>> GetOrderByBooking(Guid bookingId);
		Task<bool> CancelOrder(Guid orderId);
		Task<ApiResponse<PagedList<OrderResponse>>> GetOrders(GetOrdersPagingRequest request);
		Task<ApiResponse<PagedList<OrderResponse>>> GetOrdersByUserId(string userId, GetOrdersPagingRequest request);
		Task<ApiResponse<OrderResponse>> CreateOrderByBooking(Guid bookingId);
		Task<ApiResponse<OrderResponse>> AddOrderOrderDetail(CreateOrderDetailRequest request);
	}
}
