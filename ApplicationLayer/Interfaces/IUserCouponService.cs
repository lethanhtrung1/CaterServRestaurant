using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.UserCoupon;

namespace ApplicationLayer.Interfaces {
	public interface IUserCouponService {
		Task<ApiResponse<List<UserCouponResponse>>> GetAllCouponByUserId();
		Task<ApiResponse<UserCouponResponse>> CreateUserCoupon(Guid couponId);
		Task<bool> RemoveUserCoupon(Guid id);
	}
}
