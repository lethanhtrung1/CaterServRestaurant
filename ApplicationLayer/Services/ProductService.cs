using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Product;
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
	public class ProductService : IProductService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly Cloudinary _cloudinary;
		private readonly CloudinaryOptions _cloudinaryOptions;

		public ProductService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper,
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

		public async Task<ApiResponse<ProductResponse>> CreateAsync(CreateProductRequest request) {
			try {
				var productToDb = _mapper.Map<Product>(request);

				if (productToDb == null) {
					return new ApiResponse<ProductResponse>(false, "Internal server error occurred");
				}

				// Hanle upload image
				if (request.File != null) {
					var file = request.File;
					var uploadResult = new ImageUploadResult();
					if (file != null) {
						using (var stream = file.OpenReadStream()) {
							var uploadParam = new ImageUploadParams() {
								File = new FileDescription(file.Name, stream),
								Folder = $"{_cloudinaryOptions.Folder}/Product"
							};
							uploadResult = await _cloudinary.UploadAsync(uploadParam);
						}
					}
					// Upload success
					productToDb.Thumbnail = uploadResult.Url.ToString();
					productToDb.ThumbnailPublicId = uploadResult.PublicId;
				}

				await _unitOfWork.Product.AddAsync(productToDb);
				await _unitOfWork.SaveChangeAsync();

				var menu = await _unitOfWork.Menu.GetAsync(x => x.Id == productToDb.MenuId);
				var category = await _unitOfWork.Category.GetAsync(x => x.Id == productToDb.CategoryId);

				var productMenu = _mapper.Map<ProductMenuDto>(menu);
				var productCategory = _mapper.Map<ProductCategoryDto>(category);

				var response = _mapper.Map<ProductResponse>(productToDb);
				response.MenuDto = productMenu;
				response.CategoryDto = productCategory;

				return new ApiResponse<ProductResponse>(response, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<ProductResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			var product = await _unitOfWork.Product.GetAsync(x => x.Id == id);

			if (product == null) { return false; }

			// Handle delete image from cloudinary
			var deletionParam = new DeletionParams(product.ThumbnailPublicId) {
				ResourceType = ResourceType.Image,
			};
			var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
			if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
				_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {product.ThumbnailPublicId}");
			}

			await _unitOfWork.Product.RemoveAsync(product);
			await _unitOfWork.SaveChangeAsync();
			return true;
		}

		public async Task<ApiResponse<PagedList<ProductResponse>>> FilterAsync(FilterProductRequest request) {
			try {
				var products = await _unitOfWork.Product
					.GetListAsync(x => x.Inactive == request.InActive, includeProperties: "Menu,Category");

				if (!string.IsNullOrEmpty(request.SearchText)) {
					products = products.Where(x => request.SearchText.Contains(x.Name)).ToList();
				}

				if (request.PriceFrom > 0) {
					products = products.Where(x => x.Price >= request.PriceFrom).ToList();
				}

				if (request.PriceTo > 0) {
					products = products.Where(x => x.Price <= request.PriceTo).ToList();
				}

				if (request.MenuId != Guid.Empty) {
					products = products.Where(x => x.MenuId == request.MenuId).ToList();
				}

				if (request.CategoryId != Guid.Empty) {
					products = products.Where(x => x.CategoryId == request.CategoryId).ToList();
				}

				var productsPagedList = products.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (!productsPagedList.Any()) {
					return new ApiResponse<PagedList<ProductResponse>>(false, "No record available");
				}

				int totalRecord = products.Count();
				var productResponseDto = new List<ProductResponse>();

				foreach (var item in productsPagedList) {
					var product = _mapper.Map<ProductResponse>(item);

					product.MenuDto = _mapper.Map<ProductMenuDto>(item.Menu);
					product.CategoryDto = _mapper.Map<ProductCategoryDto>(item.Category);

					productResponseDto.Add(product);
				}

				return new ApiResponse<PagedList<ProductResponse>>(
					new PagedList<ProductResponse>(productResponseDto, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<ProductResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<ProductResponse>> GetByIdAsync(Guid id) {
			try {
				var product = await _unitOfWork.Product.GetAsync(x => x.Id == id, includeProperties: "ProductImages,Menu,Category");

				if (product == null) {
					return new ApiResponse<ProductResponse>(false, $"Product with id: {id} not found");
				}

				//var productImagesDto = _mapper.Map<List<ProductImageDto>>(product.ProductImages);
				//var productMenuDto = _mapper.Map<ProductMenuDto>(product.Menu);
				//var productCategoryDto = _mapper.Map<ProductCategoryDto>(product.Category);

				var productDto = _mapper.Map<ProductResponse>(product);

				productDto.ProductImagesDto = _mapper.Map<List<ProductImageDto>>(product.ProductImages);
				productDto.CategoryDto = _mapper.Map<ProductCategoryDto>(product.Category);
				productDto.MenuDto = _mapper.Map<ProductMenuDto>(product.Menu);

				return new ApiResponse<ProductResponse>(productDto, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<ProductResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<ProductResponse>>> GetListAsync(PagingRequest request) {
			try {
				var products = await _unitOfWork.Product.GetListAsync(includeProperties: "Menu,Category");
				var productsPagedList = products.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (!productsPagedList.Any()) {
					return new ApiResponse<PagedList<ProductResponse>>(false, "No record available");
				}

				int totalRecord = products.Count();
				var productResponseDto = new List<ProductResponse>();

				foreach (var item in productsPagedList) {
					var product = _mapper.Map<ProductResponse>(item);
					//var productMenuDto = _mapper.Map<ProductMenuDto>(item.Menu);
					//var productCategoryDto = _mapper.Map<ProductCategoryDto>(item.Category);

					product.MenuDto = _mapper.Map<ProductMenuDto>(item.Menu);
					product.CategoryDto = _mapper.Map<ProductCategoryDto>(item.Category);

					productResponseDto.Add(product);
				}

				return new ApiResponse<PagedList<ProductResponse>>(
					new PagedList<ProductResponse>(productResponseDto, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<ProductResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<ProductResponse>> UpdateAsync(UpdateProductRequest request) {
			try {
				var checkProductFromDb = await _unitOfWork.Product.GetAsync(x => x.Id == request.Id);

				if (checkProductFromDb == null) {
					return new ApiResponse<ProductResponse>(false, $"Product with id: {request.Id} not found");
				}

				_mapper.Map(request, checkProductFromDb);
				if (checkProductFromDb == null) {
					return new ApiResponse<ProductResponse>(false, "Internal server error occurred");
				}

				// Hanle upload new thumbnail
				if (request.File != null) {
					// Handle delete image from cloudinary
					var deletionParam = new DeletionParams(checkProductFromDb.ThumbnailPublicId) {
						ResourceType = ResourceType.Image,
					};
					var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
					if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
						_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {checkProductFromDb.ThumbnailPublicId}");
					}

					var file = request.File;
					var uploadResult = new ImageUploadResult();
					if (file != null) {
						using (var stream = file.OpenReadStream()) {
							var uploadParam = new ImageUploadParams() {
								File = new FileDescription(file.Name, stream),
								Folder = $"{_cloudinaryOptions.Folder}/Product"
							};
							uploadResult = await _cloudinary.UploadAsync(uploadParam);
						}
					}
					checkProductFromDb.Thumbnail = uploadResult.Url.ToString();
					checkProductFromDb.ThumbnailPublicId = uploadResult.PublicId;
				}

				await _unitOfWork.Product.UpdateAsync(checkProductFromDb);
				await _unitOfWork.SaveChangeAsync();

				var menu = await _unitOfWork.Menu.GetAsync(x => x.Id == checkProductFromDb.MenuId);
				var category = await _unitOfWork.Category.GetAsync(x => x.Id == checkProductFromDb.CategoryId);

				var response = _mapper.Map<ProductResponse>(checkProductFromDb);
				response.MenuDto = _mapper.Map<ProductMenuDto>(menu);
				response.CategoryDto = _mapper.Map<ProductCategoryDto>(category);

				return new ApiResponse<ProductResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<ProductResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
