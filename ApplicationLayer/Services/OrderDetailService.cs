using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Order;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;

namespace ApplicationLayer.Services {
	public class OrderDetailService : IOrderDetailService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ApiResponse<List<OrderDetailResponse>>> GetOrderDetail(Guid orderId) {
			try {
				var orderDetails = await _unitOfWork.OrderDetail.GetListAsync(x => x.OrderId == orderId);

				if (orderDetails == null) {
					return new ApiResponse<List<OrderDetailResponse>>(false, "Order detail not found");
				}

				var result = _mapper.Map<List<OrderDetailResponse>>(orderDetails);
				result = result.OrderByDescending(x => x.CreatedAt).ToList();
				return new ApiResponse<List<OrderDetailResponse>>(result, true, "Retrieve order detail successfully");
			} catch (Exception) {

				return new ApiResponse<List<OrderDetailResponse>>(false, "An error occurred while retrieving order details by order ID");
			}
		}

		public async Task<ApiResponse<PagedList<OrderDetailResponse>>> GetOrderDetailsPaging(PagingRequest request) {
			try {
				var orderDetails = await _unitOfWork.OrderDetail.GetListAsync();

				if (orderDetails == null) {
					return new ApiResponse<PagedList<OrderDetailResponse>>(false, "No record available");
				}

				int totalRecord = orderDetails.Count();
				var orderDetailsPagedList = orderDetails.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				var result = _mapper.Map<List<OrderDetailResponse>>(orderDetailsPagedList);

				return new ApiResponse<PagedList<OrderDetailResponse>>(
					new PagedList<OrderDetailResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception) {
				return new ApiResponse<PagedList<OrderDetailResponse>>(false, "An error occurred while retrieving order details");
			}
		}
	}
}
