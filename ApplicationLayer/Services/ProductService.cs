using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Product;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace ApplicationLayer.Services {
	public class ProductService : IProductService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ProductService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
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
					var folderName = Path.Combine("Resources", "Images");
					var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
					if (file is not null) {
						var fileName = Guid.NewGuid().ToString() + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
						var fullPath = Path.Combine(pathToSave, fileName);
						var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
						using (var stream = new FileStream(fullPath, FileMode.Create)) {
							file.CopyTo(stream);
						}

						productToDb.Thumbnail = dbPath;
					}
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

				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (!string.IsNullOrEmpty(response.Thumbnail)) {
					response.Thumbnail = $"{baseUrl}/{response.Thumbnail}";
				}

				return new ApiResponse<ProductResponse>(response, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<ProductResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			var product = await _unitOfWork.Product.GetAsync(x => x.Id == id);

			if (product == null) {
				return false;
			}

			if (!string.IsNullOrEmpty(product.Thumbnail)) {
				string imagePath = Path.Combine(Directory.GetCurrentDirectory(), product.Thumbnail);
				if (File.Exists(imagePath)) {
					File.Delete(imagePath);
				}
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

				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				foreach (var item in productsPagedList) {
					var product = _mapper.Map<ProductResponse>(item);
					var productMenuDto = _mapper.Map<ProductMenuDto>(item.Menu);
					var productCategoryDto = _mapper.Map<ProductCategoryDto>(item.Category);

					product.MenuDto = productMenuDto;
					product.CategoryDto = productCategoryDto;

					if (!string.IsNullOrEmpty(product.Thumbnail)) {
						product.Thumbnail = $"{baseUrl}/{product.Thumbnail}";
					}

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

				var productImagesDto = _mapper.Map<List<ProductImageDto>>(product.ProductImages);
				var productMenuDto = _mapper.Map<ProductMenuDto>(product.Menu);
				var productCategoryDto = _mapper.Map<ProductCategoryDto>(product.Category);
				var productDto = _mapper.Map<ProductResponse>(product);

				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (productImagesDto != null) {
					foreach (var item in productImagesDto) {
						if (!string.IsNullOrEmpty(item.ImageUrl)) {
							item.ImageUrl = $"{baseUrl}/{item.ImageUrl}";
						}
					}
				}

				productDto.ProductImagesDto = productImagesDto;
				productDto.CategoryDto = productCategoryDto;
				productDto.MenuDto = productMenuDto;

				if (!string.IsNullOrEmpty(productDto.Thumbnail)) {
					productDto.Thumbnail = $"{baseUrl}/{productDto.Thumbnail}";
				}

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

				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				foreach (var item in productsPagedList) {
					var product = _mapper.Map<ProductResponse>(item);
					var productMenuDto = _mapper.Map<ProductMenuDto>(item.Menu);
					var productCategoryDto = _mapper.Map<ProductCategoryDto>(item.Category);

					product.MenuDto = productMenuDto;
					product.CategoryDto = productCategoryDto;

					if (!string.IsNullOrEmpty(product.Thumbnail)) {
						product.Thumbnail = $"{baseUrl}/{product.Thumbnail}";
					}

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

				// remove old thumbnail
				if (!string.IsNullOrEmpty(checkProductFromDb.Thumbnail)) {
					string imagePath = Path.Combine(Directory.GetCurrentDirectory(), checkProductFromDb.Thumbnail);
					if (File.Exists(imagePath)) {
						File.Delete(imagePath);
					}
				}

				_mapper.Map(request, checkProductFromDb);
				if (checkProductFromDb == null) {
					return new ApiResponse<ProductResponse>(false, "Internal server error occurred");
				}

				// Hanle upload new thumbnail
				if (request.File != null) {
					var file = request.File;
					var folderName = Path.Combine("Resources", "Images");
					var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
					if (file is not null) {
						var fileName = Guid.NewGuid().ToString() + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
						var fullPath = Path.Combine(pathToSave, fileName);
						var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
						using (var stream = new FileStream(fullPath, FileMode.Create)) {
							file.CopyTo(stream);
						}

						checkProductFromDb.Thumbnail = dbPath;
					}
				}

				await _unitOfWork.Product.UpdateAsync(checkProductFromDb);
				await _unitOfWork.SaveChangeAsync();

				var menu = _unitOfWork.Menu.GetAsync(x => x.Id == checkProductFromDb.MenuId);
				var category = _unitOfWork.Category.GetAsync(x => x.Id == checkProductFromDb.CategoryId);

				var productMenu = _mapper.Map<ProductMenuDto>(menu);
				var productCategory = _mapper.Map<ProductCategoryDto>(category);

				var response = _mapper.Map<ProductResponse>(checkProductFromDb);
				response.MenuDto = productMenu;
				response.CategoryDto = productCategory;

				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (!string.IsNullOrEmpty(response.Thumbnail)) {
					response.Thumbnail = $"{baseUrl}/{response.Thumbnail}";
				}

				return new ApiResponse<ProductResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<ProductResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
