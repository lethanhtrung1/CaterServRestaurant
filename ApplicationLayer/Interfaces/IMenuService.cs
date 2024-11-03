using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Menu;

namespace ApplicationLayer.Interfaces {
	public interface IMenuService {
		Task<ApiResponse<MenuResponse>> GetAsync(Guid id);
		Task<ApiResponse<PagedList<MenuResponse>>> GetListAsync(PagingRequest request);
		Task<bool> DeleteAsync(Guid id);
		Task<ApiResponse<MenuResponse>> CreateAsync(CreateMenuRequest request);
		Task<ApiResponse<MenuResponse>> UpdateAsync(UpdateMenuRequest request);
	}
}
