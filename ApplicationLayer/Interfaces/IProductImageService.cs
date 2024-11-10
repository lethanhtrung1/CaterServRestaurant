using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Product;

namespace ApplicationLayer.Interfaces {
	public interface IProductImageService {
		Task<ApiResponse<List<ProductImageResponse>>> CreateAsync(CreateProductImageRequest request);
		Task<bool> DeleteAsync(Guid id);
		Task<bool> DeleteRangeAsync(Guid productId);
	}
}
