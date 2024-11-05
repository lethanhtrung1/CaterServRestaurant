using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Table;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Table;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class TableService : ITableService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;

		public TableService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResponse<TableResponse>> CreateAsync(CreateTableRequest request) {
			try {
				var table = _mapper.Map<Table>(request);

				if (table == null) {
					return new ApiResponse<TableResponse>(false, "Internal server error occurred");
				}

				await _unitOfWork.Table.AddAsync(table);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<TableResponse>(table);

				return new ApiResponse<TableResponse>(result, true, "Created successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<TableResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			var table = await _unitOfWork.Table.GetAsync(x => x.Id == id);

			if (table == null) {
				return false;
			}

			await _unitOfWork.Table.RemoveAsync(table);
			await _unitOfWork.SaveChangeAsync();
			return true;
		}

		public async Task<ApiResponse<PagedList<TableResponse>>> FilterAsync(FilterTableRequest request) {
			try {
				var tables = await _unitOfWork.Table.GetListAsync(x => x.IsAvailable == request.IsAvailable && x.MaxCapacity == request.MaxCapacity);

				if (!string.IsNullOrEmpty(request.AreaName)) {
					tables = tables.Where(x => x.AreaName == request.AreaName).ToList();
				}

				var tablesPagedList = tables.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (!tablesPagedList.Any()) {
					return new ApiResponse<PagedList<TableResponse>>(false, "No record available");
				}

				int totalRecord = tables.Count();

				var result = _mapper.Map<List<TableResponse>>(tablesPagedList);

				return new ApiResponse<PagedList<TableResponse>>(
					new PagedList<TableResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<TableResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<TableResponse>> GetByIdAsync(Guid id) {
			try {
				var table = await _unitOfWork.Table.GetAsync(x => x.Id == id);

				if (table == null) {
					return new ApiResponse<TableResponse>(false, $"Table with id: {id} not found");
				}

				var result = _mapper.Map<TableResponse>(table);

				return new ApiResponse<TableResponse>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<TableResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<TableResponse>>> GetListAsync(PagingRequest request) {
			try {
				var tables = await _unitOfWork.Table.GetListAsync();
				var tablesPagedList = tables.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (!tablesPagedList.Any()) {
					return new ApiResponse<PagedList<TableResponse>>(false, "No record available");
				}

				int totalRecord = tables.Count();
				var result = _mapper.Map<List<TableResponse>>(tablesPagedList);

				return new ApiResponse<PagedList<TableResponse>>(
					new PagedList<TableResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<TableResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<TableResponse>>> GetTableAvailableAsync(PagingRequest request) {
			try {
				var tables = await _unitOfWork.Table.GetListAsync(x => x.IsAvailable);
				var tablesPagedList = tables.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (!tablesPagedList.Any()) {
					return new ApiResponse<PagedList<TableResponse>>(false, "No record available");
				}

				int totalRecord = tables.Count();
				var result = _mapper.Map<List<TableResponse>>(tablesPagedList);

				return new ApiResponse<PagedList<TableResponse>>(
					new PagedList<TableResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<TableResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<TableResponse>> UpdateAsync(UpdateTableRequest request) {
			try {
				var checkTableExist = await _unitOfWork.Table.GetAsync(x => x.Name == request.Name);

				if (checkTableExist == null) {
					return new ApiResponse<TableResponse>(false, "Table already exist");
				}

				var table = _mapper.Map<Table>(request);

				if (table == null) {
					return new ApiResponse<TableResponse>(false, "Internal server error occurred");
				}

				await _unitOfWork.Table.UpdateAsync(table);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<TableResponse>(table);

				return new ApiResponse<TableResponse>(result, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<TableResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
