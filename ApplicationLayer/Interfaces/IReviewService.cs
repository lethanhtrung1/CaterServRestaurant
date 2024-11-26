using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Review;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Review;

namespace ApplicationLayer.Interfaces {
	public interface IReviewService {
		Task<ApiResponse<ReviewResponse>> GetReviewById(Guid Id);
		Task<ApiResponse<PagedList<ReviewResponse>>> GetPagingReviews(GetPagingReviewsRequest request);
		Task<ApiResponse<ReviewResponse>> CreateReview(CreateReviewRequest request);
		Task<ApiResponse<ReviewResponse>> UpdateReview(UpdateReviewRequest request);
		Task<bool> DeleteReview(Guid id);
		Task<ApiResponse<double?>> GetAverageRating(Guid productId);
	}
}
