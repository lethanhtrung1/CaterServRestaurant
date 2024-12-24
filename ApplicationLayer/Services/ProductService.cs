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
using DomainLayer.Caching;
using DomainLayer.Common;
using DomainLayer.Entites;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ApplicationLayer.Services {
	public class ProductService : IProductService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly Cloudinary _cloudinary;
		private readonly CloudinaryOptions _cloudinaryOptions;
		private readonly ICacheService _cacheService;

		public ProductService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper,
			IOptions<CloudinaryOptions> options, ICacheService cacheService) {
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
			_cacheService = cacheService;
		}

		public async Task<byte[]> GetTemplateExcelFile(string fileName) {
			//var fileName = "templateInsertProducts.xlsx";
			var rootPath = $"{Directory.GetCurrentDirectory()}\\wwwroot";
			var filePath = Path.Combine(rootPath, "Teamplates", fileName);

			if (!File.Exists(filePath)) { return null!; }

			return await File.ReadAllBytesAsync(filePath);
		}

		public async Task<bool> BulkInsertFromExcel(IFormFile file) {
			try {
				if (file != null && file.Length > 0) {
					var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";

					if (!Directory.Exists(uploadsFolder)) {
						Directory.CreateDirectory(uploadsFolder);
					}

					var filePath = Path.Combine(uploadsFolder, file.FileName);

					using (var stream = new FileStream(filePath, FileMode.Create)) {
						await file.CopyToAsync(stream);
					}

					await _unitOfWork.Product.BeginTransactionAsync();
					var productList = new List<Product>();
					using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read)) {
						using (var reader = ExcelReaderFactory.CreateReader(stream)) {
							bool isFirstSheet = true;
							do {
								// if not first sheet
								if (!isFirstSheet) break;

								isFirstSheet = false;
								bool isHeaderSkipped = false;

								while (reader.Read()) {
									if (!isHeaderSkipped) {
										isHeaderSkipped = true;
										continue;
									}
									if (string.IsNullOrEmpty(reader.GetValue(0)?.ToString())) { break; }

									var productDto = new Product {
										Id = Guid.NewGuid(),
										Code = reader.GetValue(0).ToString()!,
										Name = reader.GetValue(1).ToString()!,
										Description = reader.GetValue(2).ToString(),
										Price = Convert.ToInt32(reader.GetValue(3).ToString()),
										SellingPrice = Convert.ToInt32(reader.GetValue(4).ToString()),
										UnitName = reader.GetValue(5).ToString(),
										Inactive = Convert.ToInt32(reader.GetValue(6).ToString()) == 1 ? true : false,
										CategoryId = Guid.Parse(reader.GetValue(9).ToString()!),
										MenuId = Guid.Parse(reader.GetValue(10).ToString()!),
										//CategoryName = reader.GetValue(7).ToString(),
										//MenuName = reader.GetValue(8).ToString(),
										Thumbnail = "https://placehold.co/500x600/png",
										ThumbnailPublicId = string.Empty,
									};
									productList.Add(productDto);
									//await _unitOfWork.Product.AddAsync(productDto);
								}
							} while (reader.NextResult());
						}
					}
					await _unitOfWork.Product.AddRangeAsync(productList);
					await _unitOfWork.SaveChangeAsync();
					await _unitOfWork.Product.EndTransactionAsync();

					// Remove cache
					await _cacheService.RemoveMultipleKeysAsync("Products:*");

					// remove file
					if (File.Exists(filePath)) {
						File.Delete(filePath);
					}
					return true;
				}
				return false;
			} catch (Exception ex) {
				await _unitOfWork.Product.RollBackTransactionAsync();
				throw new Exception($"{ex.Message}");
			}
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

				await _cacheService.RemoveData($"Product_{productToDb.Id}");
				await _cacheService.RemoveMultipleKeysAsync("Products:*");

				var menu = await _unitOfWork.Menu.GetAsync(x => x.Id == productToDb.MenuId);
				var category = await _unitOfWork.Category.GetAsync(x => x.Id == productToDb.CategoryId);

				var productMenu = _mapper.Map<ProductMenuDto>(menu);
				var productCategory = _mapper.Map<ProductCategoryDto>(category);

				var response = _mapper.Map<ProductResponse>(productToDb);
				response.MenuDto = productMenu;
				response.CategoryDto = productCategory;

				return new ApiResponse<ProductResponse>(response, true, "Product created successfully");
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

			await _cacheService.RemoveData($"Product_{id}");
			await _cacheService.RemoveMultipleKeysAsync("Products:*");

			return true;
		}

		public async Task<ApiResponse<PagedList<ProductResponse>>> FilterAsync(FilterProductRequest request) {
			try {
				var products = await _unitOfWork.Product
					.GetListAsync(x => x.Inactive == request.InActive, includeProperties: "Menu,Category");

				if (!string.IsNullOrEmpty(request.SearchText)) {
					products = products.Where(x => x.Name.ToLower().Contains(request.SearchText.ToLower())).ToList();
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
				var cacheKey = $"Product_{id}";
				var cacheData = await _cacheService.GetData<ProductResponse>(cacheKey);
				if (cacheData != null) {
					return new ApiResponse<ProductResponse>(cacheData, true, "Retrieve products successfully");
				}

				var product = await _unitOfWork.Product.GetAsync(x => x.Id == id, includeProperties: "ProductImages,Menu,Category");

				if (product == null) {
					return new ApiResponse<ProductResponse>(false, $"Product with id: {id} not found");
				}

				var productDto = _mapper.Map<ProductResponse>(product);

				productDto.ProductImagesDto = _mapper.Map<List<ProductImageDto>>(product.ProductImages);
				productDto.CategoryDto = _mapper.Map<ProductCategoryDto>(product.Category);
				productDto.MenuDto = _mapper.Map<ProductMenuDto>(product.Menu);

				var expirationTime = DateTime.UtcNow.AddMinutes(5);
				cacheData = productDto;
				await _cacheService.SetData<ProductResponse>(cacheKey, cacheData, expirationTime);

				return new ApiResponse<ProductResponse>(cacheData, true, "Retrieve products successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<ProductResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<ProductResponse>>> GetListAsync(PagingRequest request) {
			try {
				var cacheKey = $"Products:{request.PageNumber}:{request.PageSize}";
				var cacheData = await _cacheService.GetData<PagedList<ProductResponse>>(cacheKey);

				if (cacheData != null) {
					return new ApiResponse<PagedList<ProductResponse>>(
						cacheData, true, "Retrieve products successfully"
					);
				}

				var productsPagedList = await _unitOfWork.Product
					.GetPagingAsync(x => x.Inactive, includeProperties: "Menu,Category",
					pageNumber: request.PageNumber, pageSize: request.PageSize);

				if (!productsPagedList.Item1.Any()) {
					return new ApiResponse<PagedList<ProductResponse>>(false, "No record available");
				}

				int totalRecord = productsPagedList.Item2;
				var productResponseDto = new List<ProductResponse>();

				foreach (var item in productsPagedList.Item1) {
					var product = _mapper.Map<ProductResponse>(item);

					product.MenuDto = _mapper.Map<ProductMenuDto>(item.Menu);
					product.CategoryDto = _mapper.Map<ProductCategoryDto>(item.Category);

					productResponseDto.Add(product);
				}

				var expirationTime = DateTime.UtcNow.AddMinutes(5);
				cacheData = new PagedList<ProductResponse>(productResponseDto, request.PageNumber, request.PageSize, totalRecord);
				await _cacheService.SetData<PagedList<ProductResponse>>(cacheKey, cacheData, expirationTime);

				return new ApiResponse<PagedList<ProductResponse>>(cacheData, true, "Retrieve products successfully");
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
					if (!string.IsNullOrEmpty(checkProductFromDb.ThumbnailPublicId)) {
						// Handle delete image from cloudinary
						var deletionParam = new DeletionParams(checkProductFromDb.ThumbnailPublicId) {
							ResourceType = ResourceType.Image,
						};
						var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
						if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
							_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {checkProductFromDb.ThumbnailPublicId}");
						}
					}

					// Handle upload new image to cloudinary
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

				// Update cache product id
				try {
					var productKey = $"Product_{checkProductFromDb.Id}";
					await _cacheService.SetData(productKey, checkProductFromDb,DateTimeOffset.UtcNow.AddMinutes(5));
				} catch (Exception cacheEx) {
					_logger.LogToDebugger($"Failed to update cache for coupon {request.Id}: {cacheEx.Message}");
				}
				// Remove cache paging
				await _cacheService.RemoveMultipleKeysAsync("Products:*");

				// Map response
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
