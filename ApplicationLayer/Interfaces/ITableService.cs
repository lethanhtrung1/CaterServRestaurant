using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Table;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Table;

namespace ApplicationLayer.Interfaces {
	public interface ITableService {
		Task<ApiResponse<TableResponse>> GetByIdAsync(Guid id);
		Task<ApiResponse<PagedList<TableResponse>>> GetListAsync(PagingRequest request);
		Task<ApiResponse<PagedList<TableResponse>>> FilterAsync(FilterTableRequest request);
		Task<ApiResponse<PagedList<TableResponse>>> GetTableAvailableAsync(PagingRequest request);
		Task<ApiResponse<TableResponse>> CreateAsync(CreateTableRequest request);
		Task<ApiResponse<TableResponse>> UpdateAsync(UpdateTableRequest request);
		Task<bool> DeleteAsync(Guid id);
	}
}
