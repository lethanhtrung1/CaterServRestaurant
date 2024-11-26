using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Review;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Review;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class ReviewService : IReviewService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICurrentUserService _currentUserService;
		private readonly IMapper _mapper;

		public ReviewService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper) {
			_currentUserService = currentUserService;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ApiResponse<ReviewResponse>> CreateReview(CreateReviewRequest request) {
			try {
				var product = await _unitOfWork.Product.GetAsync(x => x.Id == request.ProductId);
				if (product == null) {
					return new ApiResponse<ReviewResponse>(false, "Product not found");
				}

				var userId = _currentUserService.UserId;

				if (string.IsNullOrEmpty(userId)) {
					return new ApiResponse<ReviewResponse>(false, "An error occurred while creating rating");
				}

				var newReview = new Review {
					Id = Guid.NewGuid(),
					UserId = userId,
					ProductId = product.Id,
					Rating = request.Rating,
					ReviewDate = DateTime.Now,
					ReviewText = request.ReviewText ?? string.Empty
				};

				await _unitOfWork.Review.AddAsync(newReview);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<ReviewResponse>(newReview);
				result.UserName = _currentUserService.UserName ?? string.Empty;

				return new ApiResponse<ReviewResponse>(result, true, "Create rating success");
			} catch (Exception ex) {
				return new ApiResponse<ReviewResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteReview(Guid Id) {
			var review = await _unitOfWork.Review.GetAsync(x => x.Id == Id);
			if (review == null) { return false; }
			await _unitOfWork.Review.RemoveAsync(review);
			await _unitOfWork.SaveChangeAsync();
			return true;
		}

		public async Task<ApiResponse<double?>> GetAverageRating(Guid productId) {
			try {
				var productExist = await _unitOfWork.Product.AnyAsync(x => x.Id == productId);
				if (!productExist) {
					return new ApiResponse<double?>(false, "Product not found");
				}

				var averageRating = await _unitOfWork.Review.GetAverageRating(productId);

				if (averageRating == null) {
					return new ApiResponse<double?>(null, true, "No ratings available for this product");
				}

				return new ApiResponse<double?>(averageRating, true, "Average rating retrieved successfully");
			} catch (Exception ex) {
				return new ApiResponse<double?>(false, $"An error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<ReviewResponse>>> GetPagingReviews(GetPagingReviewsRequest request) {
			try {
				var productExist = await _unitOfWork.Product.AnyAsync(x => x.Id == request.ProductId);
				if (!productExist) {
					return new ApiResponse<PagedList<ReviewResponse>>(false, "Product not found");
				}

				var reviews = await _unitOfWork.Review.GetListAsync(x => x.ProductId == request.ProductId);
				if (reviews == null || !reviews.Any()) {
					return new ApiResponse<PagedList<ReviewResponse>>(false, "No ratings available for this product");
				}

				var reviewPaging = reviews.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
				int totalRecord = reviews.Count();

				var result = new List<ReviewResponse>();

				foreach (var item in reviewPaging) {
					var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == item.UserId);
					var reviewResponse = _mapper.Map<ReviewResponse>(item);
					reviewResponse.UserName = user.Name!;
					result.Add(reviewResponse);
				}

				return new ApiResponse<PagedList<ReviewResponse>>(
					new PagedList<ReviewResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, "Ratings retrieved successfully"
				);
			} catch (Exception ex) {
				return new ApiResponse<PagedList<ReviewResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<ReviewResponse>> GetReviewById(Guid id) {
			try {
				var review = await _unitOfWork.Review.GetAsync(x => x.Id == id);
				if (review == null) {
					return new ApiResponse<ReviewResponse>(false, "Rating not found");
				}

				var result = _mapper.Map<ReviewResponse>(review);
				var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == review.UserId);
				result.UserName = user.Name!;

				return new ApiResponse<ReviewResponse>(result, true, "Rating retrieved successfully");
			} catch (Exception ex) {
				return new ApiResponse<ReviewResponse>(false, $"An error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<ReviewResponse>> UpdateReview(UpdateReviewRequest request) {
			try {
				var review = await _unitOfWork.Review.GetAsync(x => x.Id == request.Id);
				if (review == null) {
					return new ApiResponse<ReviewResponse>(false, "Rating not found");
				}

				if (!string.IsNullOrEmpty(request.ReviewText)) {
					review.ReviewText = request.ReviewText;
				}
				review.Rating = request.Rating;

				await _unitOfWork.Review.UpdateAsync(review);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<ReviewResponse>(review);
				var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == review.UserId);
				response.UserName = user.Name!;

				return new ApiResponse<ReviewResponse>(response, true, "Update rating successfully");
			} catch (Exception ex) {
				return new ApiResponse<ReviewResponse>(false, $"An error occurred: {ex.Message}");
			}
		}
	}
}
