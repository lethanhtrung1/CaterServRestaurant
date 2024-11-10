using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Product;

namespace ApplicationLayer.Interfaces {
	public interface IProductService {
		Task<ApiResponse<ProductResponse>> GetByIdAsync(Guid id);
		Task<ApiResponse<PagedList<ProductResponse>>> GetListAsync(PagingRequest request);
		Task<ApiResponse<PagedList<ProductResponse>>> FilterAsync(FilterProductRequest request);
		Task<ApiResponse<ProductResponse>> CreateAsync(CreateProductRequest request);
		Task<ApiResponse<ProductResponse>> UpdateAsync(UpdateProductRequest request);
		Task<bool> DeleteAsync(Guid id);
	}
}
