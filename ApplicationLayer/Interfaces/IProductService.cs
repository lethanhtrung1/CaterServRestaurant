using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Product;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Interfaces {
	public interface IProductService {
		Task<ApiResponse<ProductResponse>> GetByIdAsync(Guid id);
		Task<ApiResponse<PagedList<ProductResponse>>> GetListAsync(PagingRequest request);
		Task<ApiResponse<PagedList<ProductResponse>>> FilterAsync(FilterProductRequest request);
		Task<ApiResponse<ProductResponse>> CreateAsync(CreateProductRequest request);
		Task<ApiResponse<ProductResponse>> UpdateAsync(UpdateProductRequest request);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> BulkInsertFromExcel(IFormFile file);
		Task<byte[]> GetTemplateExcelFile(string fileName);
	}
}
