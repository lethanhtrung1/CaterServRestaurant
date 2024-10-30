using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Category;

namespace ApplicationLayer.Interfaces {
	public interface ICategoryService {
		Task<ApiResponse<CategoryResponseDto>> GetByIdAsync(Guid id);
		Task<ApiResponse<CategoryResponseDto>> GetByCodeAsync(string code);
		Task<ApiResponse<PagedList<CategoryResponseDto>>> GetListAsync(PagingRequest request);
		Task<ApiResponse<CategoryResponseDto>> CreateAsync(CreateCategoryRequest request);
		Task<ApiResponse<CategoryResponseDto>> UpdateAsync(UpdateCategoryRequest request);
		Task<bool> DeleteAsync(Guid id);
	}
}
