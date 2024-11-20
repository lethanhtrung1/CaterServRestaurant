using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.PaymentSignature;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;

namespace ApplicationLayer.Services {
	public class PaymentSignatureService : IPaymentSignatureService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PaymentSignatureService(IUnitOfWork unitOfWork, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ApiResponse<PaymentSignatureResponse>> GetById(Guid id) {
			try {
				var paymentSign = await _unitOfWork.PaymentSignature.GetAsync(x => x.Id == id);

				if (paymentSign == null) {
					return new ApiResponse<PaymentSignatureResponse>(false, "Payment signature not found");
				}

				var result = _mapper.Map<PaymentSignatureResponse>(paymentSign);

				return new ApiResponse<PaymentSignatureResponse>(result, true, "");
			} catch (Exception ex) {
				return new ApiResponse<PaymentSignatureResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<PaymentSignatureResponse>>> GetPaging(PagingRequest request) {
			try {
				var payemtSigns = await _unitOfWork.PaymentSignature.GetListAsync();

				var paymentSignsPaging = payemtSigns.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (paymentSignsPaging == null || !paymentSignsPaging.Any()) {
					return new ApiResponse<PagedList<PaymentSignatureResponse>>(false, "No record available");
				}

				int totalRecord = payemtSigns.Count();
				var result = _mapper.Map<List<PaymentSignatureResponse>>(paymentSignsPaging);
				return new ApiResponse<PagedList<PaymentSignatureResponse>>(
					new PagedList<PaymentSignatureResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				return new ApiResponse<PagedList<PaymentSignatureResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
