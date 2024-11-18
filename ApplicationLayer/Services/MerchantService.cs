using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Merchant;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Merchant;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class MerchantService : IMerchantService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public MerchantService(IUnitOfWork unitOfWork, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ApiResponse<MerchantResponse>> CreateMerchant(CreateMerchantRequest request) {
			try {
				var merchant = _mapper.Map<Merchant>(request);

				if (merchant == null) {
					return new ApiResponse<MerchantResponse>(false, "");
				}
				merchant.IsActive = false;
				await _unitOfWork.Merchant.AddAsync(merchant);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<MerchantResponse>(merchant);
				return new ApiResponse<MerchantResponse>(response, true, "");
			} catch (Exception ex) {
				return new ApiResponse<MerchantResponse>(false, $"An error occurred while creating merchant: {ex.Message}");
			}
		}

		public async Task<bool> Delete(Guid id) {
			try {
				var merchant = await _unitOfWork.Merchant.GetAsync(x => x.Id == id);

				if (merchant == null) { return false; }

				await _unitOfWork.Merchant.RemoveAsync(merchant);
				await _unitOfWork.SaveChangeAsync();

				return true;
			} catch (Exception ex) {
				throw new Exception("An error occurred while deleting merchant", ex);
			}
		}

		public async Task<ApiResponse<MerchantResponse>> GetById(Guid id) {
			try {
				var merchant = await _unitOfWork.Merchant.GetAsync(x => x.Id == id);

				if (merchant == null) {
					return new ApiResponse<MerchantResponse>(false, $"Merchant with Id: {id} not found");
				}

				var response = _mapper.Map<MerchantResponse>(merchant);
				return new ApiResponse<MerchantResponse>(response, true, "");
			} catch (Exception ex) {
				return new ApiResponse<MerchantResponse>(false, $"An error occurred while retrieving the merchant by Id: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<MerchantResponse>>> GetPaging(GetMerchantPaging request) {
			try {
				var merchants = await _unitOfWork.Merchant.GetListAsync();

				if (request.IsActive == 1) {
					merchants = merchants.Where(x => x.IsActive);
				} else if (request.IsActive == -1) {
					merchants = merchants.Where(x => !x.IsActive);
				}

				if (merchants == null || !merchants.Any()) {
					return new ApiResponse<PagedList<MerchantResponse>>(false, "No record available");
				}
				int totalRecord = merchants.Count();
				var merchantsPaging = merchants.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				var response = _mapper.Map<List<MerchantResponse>>(merchantsPaging);

				return new ApiResponse<PagedList<MerchantResponse>>(
					new PagedList<MerchantResponse>(response, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				return new ApiResponse<PagedList<MerchantResponse>>(false, $"An error occurred while retrieving the merchants: {ex.Message}");
			}
		}

		public async Task<ApiResponse<MerchantResponse>> SetActive(SetActiveRequest request) {
			try {
				var merchant = await _unitOfWork.Merchant.GetAsync(x => x.Id == request.Id);

				if (merchant == null) {
					return new ApiResponse<MerchantResponse>(false, $"Merchant with Id: {request.Id} not found");
				}

				merchant.IsActive = request.IsActive;
				await _unitOfWork.Merchant.UpdateAsync(merchant);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<MerchantResponse>(merchant);

				return new ApiResponse<MerchantResponse>(response, true, "");
			} catch (Exception ex) {
				return new ApiResponse<MerchantResponse>(false, $"An error occurred while updating the merchant: {ex.Message}");
			}
		}

		public async Task<ApiResponse<MerchantResponse>> UpdateMerchant(UpdateMerchantRequest request) {
			try {
				var merchant = await _unitOfWork.Merchant.GetAsync(x => x.Id == request.Id);

				if (merchant == null) {
					return new ApiResponse<MerchantResponse>(false, $"Merchant with Id: {request.Id} not found");
				}

				_mapper.Map(request, merchant);

				await _unitOfWork.Merchant.UpdateAsync(merchant);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<MerchantResponse>(merchant);

				return new ApiResponse<MerchantResponse>(response, true, "");
			} catch (Exception ex) {
				return new ApiResponse<MerchantResponse>(false, $"An error occurred while updating the merchant: {ex.Message}");
			}
		}
	}
}
