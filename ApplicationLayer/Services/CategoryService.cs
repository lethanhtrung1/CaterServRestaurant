using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Category;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Category;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class CategoryService : ICategoryService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;

		public CategoryService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResponse<CategoryResponseDto>> CreateAsync(CreateCategoryRequest request) {
			try {
				var categoryFromDb = await _unitOfWork.Category.GetAsync(x => x.Code == request.Code);

				if (categoryFromDb != null) {
					return new ApiResponse<CategoryResponseDto>(false, $"Category with code: {request.Code} already exits");
				}

				var categoryEntity = _mapper.Map<Category>(request);

				if (categoryEntity == null) {
					return new ApiResponse<CategoryResponseDto>(false, "Internal server error occurred");
				}

				await _unitOfWork.Category.AddAsync(categoryEntity);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<CategoryResponseDto>(categoryEntity);
				return new ApiResponse<CategoryResponseDto>(result, true, "Created successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CategoryResponseDto>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			try {
				var category = await _unitOfWork.Category.GetAsync(x => x.Id == id);

				if (category == null) {
					return false;
				}

				await _unitOfWork.Category.RemoveAsync(category);
				await _unitOfWork.SaveChangeAsync();
				return true;
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				throw new Exception(ex.Message);
			}
		}

		public async Task<ApiResponse<CategoryResponseDto>> GetByCodeAsync(string code) {
			try {
				var category = await _unitOfWork.Category.GetAsync(x => x.Code == code);

				if (category == null) {
					return new ApiResponse<CategoryResponseDto>(false, "Category not found");
				}

				var result = _mapper.Map<CategoryResponseDto>(category);

				return new ApiResponse<CategoryResponseDto>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CategoryResponseDto>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CategoryResponseDto>> GetByIdAsync(Guid id) {
			try {
				var category = await _unitOfWork.Category.GetAsync(x => x.Id == id);

				if (category == null) {
					return new ApiResponse<CategoryResponseDto>(false, "Category not found");
				}

				var result = _mapper.Map<CategoryResponseDto>(category);

				return new ApiResponse<CategoryResponseDto>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CategoryResponseDto>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<CategoryResponseDto>>> GetListAsync(PagingRequest request) {
			try {
				var categories = await _unitOfWork.Category.GetListAsync();
				var categoriesPageList = categories.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (categoriesPageList is null || categoriesPageList.Count() == 0) {
					return new ApiResponse<PagedList<CategoryResponseDto>>(false, "No record available");
				}

				int totalRecord = categories.Count();
				var result = _mapper.Map<List<CategoryResponseDto>>(categoriesPageList);

				return new ApiResponse<PagedList<CategoryResponseDto>>(
						new PagedList<CategoryResponseDto>(result, request.PageNumber, request.PageSize, totalRecord),
						true,
						""
					);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<CategoryResponseDto>>(false, ex.Message);
			}
		}

		public async Task<ApiResponse<CategoryResponseDto>> UpdateAsync(UpdateCategoryRequest request) {
			try {
				var categoryFromDb = await _unitOfWork.Category.GetAsync(x => x.Id == request.Id);

				if (categoryFromDb != null) {
					return new ApiResponse<CategoryResponseDto>(false, $"Category with Id: {request.Id} already exits");
				}

				var categoryEntity = _mapper.Map<Category>(request);

				if (categoryEntity == null) {
					return new ApiResponse<CategoryResponseDto>(false, "Internal server error occurred");
				}

				await _unitOfWork.Category.UpdateAsync(categoryEntity);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<CategoryResponseDto>(categoryEntity);
				return new ApiResponse<CategoryResponseDto>(result, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CategoryResponseDto>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
