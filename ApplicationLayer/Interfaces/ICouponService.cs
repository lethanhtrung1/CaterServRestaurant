using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Coupon;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Coupon;

namespace ApplicationLayer.Interfaces {
	public interface ICouponService {
		Task<ApiResponse<CouponResponse>> GetByIdAsync(Guid id);
		Task<ApiResponse<CouponResponse>> GetCouponActiveByIdAsync(Guid id);
		Task<ApiResponse<CouponResponse>> GetByCodeAsync(string code);
		Task<ApiResponse<PagedList<CouponResponse>>> GetListAsync(PagingRequest request);
		Task<ApiResponse<PagedList<CouponResponse>>> GetListCouponActiveAsync(PagingRequest request);
		Task<ApiResponse<CouponResponse>> CreateAsync(CreateCouponRequest request);
		Task<ApiResponse<CouponResponse>> UpdateAsync(UpdateCouponRequest request);
		Task<bool> DeleteAsync(Guid id);
	}
}
