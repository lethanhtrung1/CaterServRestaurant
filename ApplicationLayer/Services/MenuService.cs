using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Menu;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Menu;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using ApplicationLayer.Options;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.Extensions.Options;

namespace ApplicationLayer.Services {
	public class MenuService : IMenuService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly Cloudinary _cloudinary;
		private readonly CloudinaryOptions _cloudinaryOptions;

		public MenuService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper,
			IOptions<CloudinaryOptions> options) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_cloudinaryOptions = options.Value;
			Account account = new Account(
				_cloudinaryOptions.CloudName,
				_cloudinaryOptions.ApiKey,
				_cloudinaryOptions.ApiSecret
			);
			_cloudinary = new Cloudinary(account);
		}

		public async Task<ApiResponse<MenuResponse>> CreateAsync(CreateMenuRequest request) {
			try {
				var menuToDb = _mapper.Map<Menu>(request);
				menuToDb.IsDeleted = false;

				// Hanle upload image
				var file = request.File;
				var uploadResult = new ImageUploadResult();
				if (file != null) {
					using (var stream = file.OpenReadStream()) {
						var uploadParam = new ImageUploadParams() {
							File = new FileDescription(file.Name, stream),
							Folder = $"{_cloudinaryOptions.Folder}/Menu"
						};
						uploadResult = await _cloudinary.UploadAsync(uploadParam);
					}

					menuToDb.ImageUrl = uploadResult.Url.ToString();
					menuToDb.MenuPublicId = uploadResult.PublicId;
				}

				await _unitOfWork.Menu.AddAsync(menuToDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<MenuResponse>(menuToDb);

				return new ApiResponse<MenuResponse>(result, true, "Created successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<MenuResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			try {
				var menu = await _unitOfWork.Menu.GetAsync(x => x.Id == id);

				if (menu == null) { return false; }

				//// Handle delete image from cloudinary
				//var deletionParam = new DeletionParams(menu.MenuPublicId) {
				//	ResourceType = ResourceType.Image,
				//};
				//var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
				//if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
				//	_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {menu.MenuPublicId}");
				//}

				menu.IsDeleted = true;
				await _unitOfWork.Menu.UpdateAsync(menu);
				await _unitOfWork.SaveChangeAsync();

				return true;
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				throw new Exception(ex.Message);
			}
		}

		public async Task<ApiResponse<MenuResponse>> GetAsync(Guid id) {
			try {
				var menu = await _unitOfWork.Menu.GetAsync(x => x.Id == id && x.Inactive == false);

				if (menu is null) {
					return new ApiResponse<MenuResponse>(false, $"Menu with Id: {id} not exist");
				}

				var result = _mapper.Map<MenuResponse>(menu);

				return new ApiResponse<MenuResponse>(result, true, "Retrieve menu successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<MenuResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<MenuResponse>>> GetListAsync(PagingRequest request) {
			try {
				var menus = await _unitOfWork.Menu.GetListAsync(x => x.IsDeleted == false);
				var menuPagedList = menus.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (menuPagedList is null || !menuPagedList.Any()) {
					return new ApiResponse<PagedList<MenuResponse>>(false, "No record available");
				}

				int totalRecord = menus.Count();
				var result = _mapper.Map<List<MenuResponse>>(menuPagedList);

				return new ApiResponse<PagedList<MenuResponse>>
					(new PagedList<MenuResponse>(result, request.PageNumber, request.PageSize, totalRecord), true, "Retrieve menus successfully");

			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<MenuResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<List<MenuResponse>>> GetListActiveAsync() {
			try {
				var menus = await _unitOfWork.Menu.GetListAsync(x => x.Inactive == true && x.IsDeleted == false);

				if (menus is null || !menus.Any()) {
					return new ApiResponse<List<MenuResponse>>(false, "No record available");
				}

				var result = _mapper.Map<List<MenuResponse>>(menus);

				return new ApiResponse<List<MenuResponse>>(result, true, "Retrieve menus active successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<List<MenuResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<MenuResponse>> UpdateAsync(UpdateMenuRequest request) {
			try {
				var menuFromDb = await _unitOfWork.Menu.GetAsync(x => x.Id == request.Id && x.IsDeleted == false);

				if (menuFromDb is null) {
					return new ApiResponse<MenuResponse>(false, $"Menu with Id: {request.Id} not exist");
				}

				menuFromDb.MenuName = request.MenuName!;
				menuFromDb.Description = request.Description!;
				menuFromDb.Inactive = request.Inactive;
				menuFromDb.SortOrder = request.SortOrder;

				// Hanle upload image
				var file = request.File;
				if (file is not null) {
					// Handle delete image from cloudinary
					var deletionParam = new DeletionParams(menuFromDb.MenuPublicId) {
						ResourceType = ResourceType.Image,
					};
					var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
					if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
						_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {menuFromDb.MenuPublicId}");
					}

					// Upload new image
					var uploadResult = new ImageUploadResult();
					using (var stream = file.OpenReadStream()) {
						var uploadParam = new ImageUploadParams() {
							File = new FileDescription(file.Name, stream),
							Folder = $"{_cloudinaryOptions.Folder}/Menu"
						};
						uploadResult = await _cloudinary.UploadAsync(uploadParam);
					}
					menuFromDb.ImageUrl = uploadResult.Url.ToString();
					menuFromDb.MenuPublicId = uploadResult.PublicId;
				}

				// save to db
				await _unitOfWork.Menu.UpdateAsync(menuFromDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<MenuResponse>(menuFromDb);

				return new ApiResponse<MenuResponse>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<MenuResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
