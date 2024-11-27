using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Product;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using DomainLayer.Common;
using DomainLayer.Entites;
using ApplicationLayer.Options;
using Microsoft.Extensions.Options;

namespace ApplicationLayer.Services {
	public class ProductImageService : IProductImageService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly Cloudinary _cloudinary;
		private readonly CloudinaryOptions _cloudinaryOptions;

		public ProductImageService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper,
			 IOptions<CloudinaryOptions> options
		) {
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

		public async Task<ApiResponse<List<ProductImageResponse>>> CreateAsync(CreateProductImageRequest request) {
			try {
				var product = await _unitOfWork.Product.GetAsync(x => x.Id == request.ProductId);

				if (product == null) {
					return new ApiResponse<List<ProductImageResponse>>(false, $"Product with id: {request.ProductId} not found");
				}

				if (request.Files.Any(f => f.Length == 0)) {
					return new ApiResponse<List<ProductImageResponse>>(false, "Invalid request files");
				}

				var productImagesResponse = new List<ProductImageResponse>();

				foreach (var file in request.Files) {
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

					var productImage = new ProductImage {
						Id = Guid.NewGuid(),
						ProductId = request.ProductId,
						ImageUrl = uploadResult.Url.ToString(),
						PublicId = uploadResult.PublicId,
					};

					await _unitOfWork.ProductImage.AddAsync(productImage);

					var productImageDto = _mapper.Map<ProductImageResponse>(productImage);
					productImagesResponse.Add(productImageDto);
				}

				await _unitOfWork.SaveChangeAsync();

				return new ApiResponse<List<ProductImageResponse>>(productImagesResponse, true, "Upload images successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<List<ProductImageResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			var productImage = await _unitOfWork.ProductImage.GetAsync(x => x.Id == id);
			if (productImage == null) return false;

			// Handle delete image from cloudinary
			var deletionParam = new DeletionParams(productImage.PublicId) {
				ResourceType = ResourceType.Image,
			};
			var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
			if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
				_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {productImage.PublicId}");
			}

			await _unitOfWork.ProductImage.RemoveAsync(productImage);
			await _unitOfWork.SaveChangeAsync();
			return true;
		}

		public async Task<bool> DeleteRangeAsync(Guid productId) {
			var productImages = await _unitOfWork.ProductImage.GetListAsync(x => x.ProductId == productId);
			if (productImages == null) return false;
			foreach (var item in productImages) {
				// Handle delete image from cloudinary
				var deletionParam = new DeletionParams(item.PublicId) {
					ResourceType = ResourceType.Image,
				};
				var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
				if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
					_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {item.PublicId}");
				}

				await _unitOfWork.ProductImage.RemoveAsync(item);
			}
			await _unitOfWork.SaveChangeAsync();
			return true;
		}
	}
}
