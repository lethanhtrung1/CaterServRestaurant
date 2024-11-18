using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.PaymetDestination;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.PaymentDestination;

namespace ApplicationLayer.Interfaces {
	public interface IPaymentDestinationService {
		Task<ApiResponse<PaymentDestinationResponse>> GetById(Guid id);
		Task<ApiResponse<PagedList<PaymentDestinationResponse>>> GetPaging(GetPaymentDesPagingRequest request);
		Task<ApiResponse<PaymentDestinationResponse>> Create(CreatePaymentDestinationRequest request);
		Task<ApiResponse<PaymentDestinationResponse>> Update(UpdatePaymentDestinationRequest request);
		Task<ApiResponse<PaymentDestinationResponse>> SetActive(SetActivePaymentDestinationRequest request);
		Task<bool> Delete(Guid id);
	}
}
