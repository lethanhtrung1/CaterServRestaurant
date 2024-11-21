using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Menu;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace ApplicationLayer.Services {
	public class MenuService : IMenuService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public MenuService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<ApiResponse<MenuResponse>> CreateAsync(CreateMenuRequest request) {
			try {
				var menuToDb = _mapper.Map<Menu>(request);

				// Hanle upload image
				var file = request.File;
				var folderName = Path.Combine("Resources", "Images");
				var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
				if (file is not null) {
					var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
					var fullPath = Path.Combine(pathToSave, fileName);
					var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
					using (var stream = new FileStream(fullPath, FileMode.Create)) {
						file.CopyTo(stream);
					}

					menuToDb.ImageUrl = dbPath;
				}

				await _unitOfWork.Menu.AddAsync(menuToDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<MenuResponse>(menuToDb);
				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (result.ImageUrl != null) {
					result.ImageUrl = $"{baseUrl}/{result.ImageUrl}";
				}

				return new ApiResponse<MenuResponse>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<MenuResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			try {
				var menu = await _unitOfWork.Menu.GetAsync(x => x.Id == id);

				if (menu == null) {
					return false;
				}

				// remove image
				if (menu.ImageUrl != null) {
					string imagePath = Path.Combine(Directory.GetCurrentDirectory(), menu.ImageUrl);
					if (File.Exists(imagePath)) {
						File.Delete(imagePath);
					}
				}

				await _unitOfWork.Menu.RemoveAsync(menu);
				await _unitOfWork.SaveChangeAsync();

				return true;
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				throw new Exception(ex.Message);
			}
		}

		public async Task<ApiResponse<MenuResponse>> GetAsync(Guid id) {
			try {
				var menu = await _unitOfWork.Menu.GetAsync(x => x.Id == id);

				if (menu is null) {
					return new ApiResponse<MenuResponse>(false, $"Menu with Id: {id} not exist");
				}

				var result = _mapper.Map<MenuResponse>(menu);
				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (result.ImageUrl != null) {
					result.ImageUrl = $"{baseUrl}/{result.ImageUrl}";
				}

				return new ApiResponse<MenuResponse>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<MenuResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<MenuResponse>>> GetListAsync(PagingRequest request) {
			try {
				var menus = await _unitOfWork.Menu.GetListAsync();
				var menuPagedList = menus.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (menuPagedList is null || !menuPagedList.Any()) {
					return new ApiResponse<PagedList<MenuResponse>>(false, "No record available");
				}

				int totalRecord = menus.Count();
				var result = _mapper.Map<List<MenuResponse>>(menuPagedList);
				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				foreach (var item in result) {
					if (item.ImageUrl != null) {
						item.ImageUrl = $"{baseUrl}/{item.ImageUrl}";
					}
				}

				return new ApiResponse<PagedList<MenuResponse>>
					(new PagedList<MenuResponse>(result, request.PageNumber, request.PageSize, totalRecord), true, "");
				
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<MenuResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<List<MenuResponse>>> GetListActiveAsync() {
			try {
				var menus = await _unitOfWork.Menu.GetListAsync(x => x.Inactive);

				if (menus is null || !menus.Any()) {
					return new ApiResponse<List<MenuResponse>>(false, "No record available");
				}

				var result = _mapper.Map<List<MenuResponse>>(menus);
				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				foreach (var item in result) {
					if (item.ImageUrl != null) {
						item.ImageUrl = $"{baseUrl}/{item.ImageUrl}";
					}
				}

				return new ApiResponse<List<MenuResponse>>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<List<MenuResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<MenuResponse>> UpdateAsync(UpdateMenuRequest request) {
			try {
				var menuFromDb = await _unitOfWork.Menu.GetAsync(x => x.Id == request.Id);

				if (menuFromDb is null) {
					return new ApiResponse<MenuResponse>(false, $"Menu with Id: {request.Id} not exist");
				}

				menuFromDb.MenuName = request.MenuName!;
				menuFromDb.Description = request.Description!;
				menuFromDb.Inactive = request.Inactive;
				menuFromDb.SortOrder = request.SortOrder;

				// Hanle upload image
				var file = request.File;
				var folderName = Path.Combine("Resources", "Images");
				var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
				if (file is not null) {
					// Handle remove image
					if (menuFromDb.ImageUrl != null) {
						string imagePath = Path.Combine(Directory.GetCurrentDirectory(), menuFromDb.ImageUrl);
						if (File.Exists(imagePath)) {
							File.Delete(imagePath);
						}
					}

					var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
					var fullPath = Path.Combine(pathToSave, fileName);
					var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
					using (var stream = new FileStream(fullPath, FileMode.Create)) {
						file.CopyTo(stream);
					}

					menuFromDb.ImageUrl = dbPath;
				}

				// save to db
				await _unitOfWork.Menu.UpdateAsync(menuFromDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<MenuResponse>(menuFromDb);
				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (result.ImageUrl != null) {
					result.ImageUrl = $"{baseUrl}/{result.ImageUrl}";
				}

				return new ApiResponse<MenuResponse>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<MenuResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
