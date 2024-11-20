using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.PaymentSignature;

namespace ApplicationLayer.Interfaces {
	public interface IPaymentSignatureService {
		Task<ApiResponse<PaymentSignatureResponse>> GetById(Guid id);
		Task<ApiResponse<PagedList<PaymentSignatureResponse>>> GetPaging(PagingRequest request);
	}
}
