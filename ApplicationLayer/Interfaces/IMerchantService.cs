using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Merchant;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Merchant;

namespace ApplicationLayer.Interfaces {
	public interface IMerchantService {
		Task<ApiResponse<MerchantResponse>> CreateMerchant(CreateMerchantRequest request);
		Task<ApiResponse<MerchantResponse>> UpdateMerchant(UpdateMerchantRequest request);
		Task<ApiResponse<MerchantResponse>> GetById(Guid id);
		Task<ApiResponse<PagedList<MerchantResponse>>> GetPaging(GetMerchantPaging request);
		Task<ApiResponse<MerchantResponse>> SetActive(SetActiveRequest request);
		Task<bool> Delete(Guid id);
	}
}
