using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.PaymetDestination;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.PaymentDestination;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class PaymentDestinationService : IPaymentDestinationService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PaymentDestinationService(IUnitOfWork unitOfWork, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ApiResponse<PaymentDestinationResponse>> Create(CreatePaymentDestinationRequest request) {
			try {
				var paymentDestination = _mapper.Map<PaymentDestination>(request);

				if (paymentDestination == null) {
					return new ApiResponse<PaymentDestinationResponse>(false, "An error occurred while creating payment destination");
				}
				paymentDestination.IsActive = false;
				await _unitOfWork.PaymentsDestination.AddAsync(paymentDestination);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<PaymentDestinationResponse>(paymentDestination);
				return new ApiResponse<PaymentDestinationResponse>(response, true, "");
			} catch (Exception ex) {
				return new ApiResponse<PaymentDestinationResponse>(false, $"An error occurred while creating payment destination: {ex.Message}");
			}
		}

		public async Task<bool> Delete(Guid id) {
			var paymentDestination = await _unitOfWork.PaymentsDestination.GetAsync(x => x.Id == id);

			if (paymentDestination == null) {
				return false;
			}

			await _unitOfWork.PaymentsDestination.RemoveAsync(paymentDestination);
			await _unitOfWork.SaveChangeAsync();

			return true;
		}

		public async Task<ApiResponse<PaymentDestinationResponse>> GetById(Guid id) {
			try {
				var paymentDes = await _unitOfWork.PaymentsDestination.GetAsync(x => x.Id == id);

				if (paymentDes == null) {
					return new ApiResponse<PaymentDestinationResponse>(false, $"Payment destination with id: {id} not found");
				}

				var response = _mapper.Map<PaymentDestinationResponse>(paymentDes);

				return new ApiResponse<PaymentDestinationResponse>(response, true, "");
			} catch (Exception ex) {
				return new ApiResponse<PaymentDestinationResponse>(false, $"An error occurred while retrieving payment destination: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<PaymentDestinationResponse>>> GetPaging(GetPaymentDesPagingRequest request) {
			try {
				var paymentDestinations = await _unitOfWork.PaymentsDestination.GetListAsync();

				if (request.IsActive == 1) {
					paymentDestinations = paymentDestinations.Where(x => x.IsActive);
				} else if (request.IsActive == -1) {
					paymentDestinations = paymentDestinations.Where(x => !x.IsActive);
				}

				if (paymentDestinations == null || !paymentDestinations.Any()) {
					return new ApiResponse<PagedList<PaymentDestinationResponse>>(false, "No record available");
				}

				int totalRecord = paymentDestinations.Count();
				var paymentDesPaging = paymentDestinations.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				var response = _mapper.Map<List<PaymentDestinationResponse>>(paymentDesPaging);

				return new ApiResponse<PagedList<PaymentDestinationResponse>>(
					new PagedList<PaymentDestinationResponse>(response, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				return new ApiResponse<PagedList<PaymentDestinationResponse>>(false, $"An error occurred while retrieving payment destination: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PaymentDestinationResponse>> SetActive(SetActivePaymentDestinationRequest request) {
			try {
				var paymetDes = await _unitOfWork.PaymentsDestination.GetAsync(x => x.Id == request.Id);

				if (paymetDes == null) {
					return new ApiResponse<PaymentDestinationResponse>(false, $"Payment desitnation with id: {request.Id} not found");
				}

				paymetDes.IsActive = request.IsActive;
				await _unitOfWork.PaymentsDestination.UpdateAsync(paymetDes);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<PaymentDestinationResponse>(paymetDes);

				return new ApiResponse<PaymentDestinationResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {
				return new ApiResponse<PaymentDestinationResponse>(false, $"An error occurred while updating payment destination: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PaymentDestinationResponse>> Update(UpdatePaymentDestinationRequest request) {
			try {
				var paymetDes = await _unitOfWork.PaymentsDestination.GetAsync(x => x.Id == request.Id);

				if (paymetDes == null) {
					return new ApiResponse<PaymentDestinationResponse>(false, $"Payment desitnation with id: {request.Id} not found");
				}

				_mapper.Map(request, paymetDes);
				await _unitOfWork.PaymentsDestination.UpdateAsync(paymetDes);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<PaymentDestinationResponse>(paymetDes);

				return new ApiResponse<PaymentDestinationResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {
				return new ApiResponse<PaymentDestinationResponse>(false, $"An error occurred while updating payment destination: {ex.Message}");
			}
		}
	}
}
