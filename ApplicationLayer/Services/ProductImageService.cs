using ApplicationLayer.DTOs.Requests.Product;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Product;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using System.Net.Http.Headers;

namespace ApplicationLayer.Services {
	public class ProductImageService : IProductImageService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;

		public ProductImageService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
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

				var folderName = Path.Combine("Resources", "Images");
				var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
				foreach (var file in request.Files) {
					var fileName = Guid.NewGuid().ToString() + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
					var fullPath = Path.Combine(pathToSave, fileName);
					var dbPath = Path.Combine(folderName, fileName);

					var productImage = new ProductImage {
						Id = Guid.NewGuid(),
						ProductId = request.ProductId,
						ImageUrl = dbPath,
					};

					await _unitOfWork.ProductImage.AddAsync(productImage);

					var productImageDto = _mapper.Map<ProductImageResponse>(productImage);
					productImagesResponse.Add(productImageDto);
				}

				await _unitOfWork.SaveChangeAsync();

				return new ApiResponse<List<ProductImageResponse>>(productImagesResponse, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<List<ProductImageResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			var productImage = await _unitOfWork.ProductImage.GetAsync(x => x.Id == id);
			if (productImage == null) return false;

			if (!string.IsNullOrEmpty(productImage.ImageUrl)) {
				string imagePath = Path.Combine(Directory.GetCurrentDirectory(), productImage.ImageUrl);
				if (File.Exists(imagePath)) {
					File.Delete(imagePath);
				}
			}

			await _unitOfWork.ProductImage.RemoveAsync(productImage);
			await _unitOfWork.SaveChangeAsync();
			return true;
		}

		public async Task<bool> DeleteRangeAsync(Guid productId) {
			var productImages = await _unitOfWork.ProductImage.GetListAsync(x => x.ProductId == productId);
			if (productImages == null) return false;
			foreach (var item in productImages) {
				if (!string.IsNullOrEmpty(item.ImageUrl)) {
					string imagePath = Path.Combine(Directory.GetCurrentDirectory(), item.ImageUrl);
					if (File.Exists(imagePath)) {
						File.Delete(imagePath);
					}
				}

				await _unitOfWork.ProductImage.RemoveAsync(item);
			}
			await _unitOfWork.SaveChangeAsync();
			return true;
		}
	}
}
