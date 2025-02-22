﻿using ApplicationLayer.Common.Constants;
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
				table.IsAvailable = true;
				table.Status = TableStatus.Free;

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
				var tables = await _unitOfWork.Table.GetListAsync();

				if (request.IsAvailable) {
					tables = tables.Where(x => x.IsAvailable).ToList();
				}

				if (!string.IsNullOrEmpty(request.Status)) {
					tables = tables.Where(x => x.Status == request.Status).ToList();
				}

				if (request.MaxCapacity > 0) {
					tables = tables.Where(x => x.MaxCapacity == request.MaxCapacity).ToList();
				}

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
				var checkTableExist = await _unitOfWork.Table.GetAsync(x => x.Id == request.Id);

				if (checkTableExist == null) {
					return new ApiResponse<TableResponse>(false, "Table already exist");
				}

				_mapper.Map(request, checkTableExist);

				if (checkTableExist == null) {
					return new ApiResponse<TableResponse>(false, "Internal server error occurred");
				}

				await _unitOfWork.Table.UpdateAsync(checkTableExist);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<TableResponse>(checkTableExist);

				return new ApiResponse<TableResponse>(result, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<TableResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<TableResponse>> UpdateStatusAsync(UpdateTableStatus request) {
			try {
				var checkTableExist = await _unitOfWork.Table.GetAsync(x => x.Id == request.TableId);

				if (checkTableExist == null) {
					return new ApiResponse<TableResponse>(false, "Table already exist");
				}

				if (request.Status == "Free") checkTableExist.Status = TableStatus.Free;
				else if (request.Status == "Reverved") checkTableExist.Status = TableStatus.Reverved;
				else if (request.Status == "Occupied") checkTableExist.Status = TableStatus.Occupied;
				else checkTableExist.Status = TableStatus.Other;

				await _unitOfWork.Table.UpdateAsync(checkTableExist);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<TableResponse>(checkTableExist);

				return new ApiResponse<TableResponse>(result, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<TableResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
