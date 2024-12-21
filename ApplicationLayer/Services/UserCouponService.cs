using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.UserCoupon;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Caching;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class UserCouponService : IUserCouponService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly ICacheService _cacheService;

		public UserCouponService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, ICacheService cacheService) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_currentUserService = currentUserService;
			_cacheService = cacheService;
		}

		public async Task<ApiResponse<UserCouponResponse>> CreateUserCoupon(Guid couponId) {
			try {
				await _unitOfWork.UserCoupon.BeginTransactionAsync();
				var userId = _currentUserService.UserId;

				if (string.IsNullOrEmpty(userId)) {
					throw new UnauthorizedAccessException();
				}

				var couponKey = $"coupon:{couponId}:quantity";
				// Get coupon quantity from Redis
				var currentQuantity = await _cacheService.GetData<int>(couponKey);
				if (currentQuantity <= 0) {
					return new ApiResponse<UserCouponResponse>(false, "Coupon out of stock");
				}

				// Decrement coupon quantity atomically in Redis
				bool isDecremented = await _cacheService.SetData(couponKey, currentQuantity - 1, DateTimeOffset.UtcNow.AddMinutes(10));
				if (!isDecremented) {
					return new ApiResponse<UserCouponResponse>(false, "Failed to reserve coupon. Try again.");
				}

				// Validate coupon in database as fallback
				var checkCoupon = await _unitOfWork.Coupon.GetAsync(x => x.Id == couponId && x.Inactive);
				if (checkCoupon == null || checkCoupon.Quantity <= 0) {
					return new ApiResponse<UserCouponResponse>(false, "Invalid coupon request or no coupons left");
				}

				var checkUserCoupon = await _unitOfWork.UserCoupon.GetAsync(x => x.CouponId == couponId && x.UserId == userId);
				if (checkUserCoupon != null) {
					return new ApiResponse<UserCouponResponse>(false, "You already own this coupon");
				}

				// Decrement database quantity
				checkCoupon.Quantity--;
				await _unitOfWork.Coupon.UpdateAsync(checkCoupon);

				// Create UserCoupon
				var newUserCoupon = new UserCoupon {
					Id = Guid.NewGuid(),
					CouponId = couponId,
					UserId = userId
				};

				await _unitOfWork.UserCoupon.AddAsync(newUserCoupon);
				await _unitOfWork.SaveChangeAsync();
				await _unitOfWork.UserCoupon.EndTransactionAsync();

				// Map response
				var response = _mapper.Map<UserCouponResponse>(newUserCoupon);
				response.CouponCode = checkCoupon.CouponCode;
				response.DiscountPercent = checkCoupon.DiscountPercent;
				response.DiscountAmount = checkCoupon.DiscountAmount;

				return new ApiResponse<UserCouponResponse>();
			} catch (Exception ex) {
				await _unitOfWork.UserCoupon.RollBackTransactionAsync();
				return new ApiResponse<UserCouponResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<List<UserCouponResponse>>> GetAllCouponByUserId() {
			try {
				var userId = _currentUserService.UserId;
				var userCoupons = await _unitOfWork.UserCoupon.GetListAsync(x => x.UserId == userId, includeProperties: "Coupon");

				var userCouponsIsActive = userCoupons.Where(x => x.Coupon.Inactive).ToList();

				if (userCouponsIsActive == null || userCouponsIsActive.Count == 0) {
					return new ApiResponse<List<UserCouponResponse>>(false, "No record available");
				}

				var response = new List<UserCouponResponse>();
				foreach (var userCoupon in userCouponsIsActive) {
					var userCouponResponse = _mapper.Map<UserCouponResponse>(userCoupon);
					userCouponResponse.CouponCode = userCoupon.Coupon.CouponCode;
					userCouponResponse.DiscountPercent = userCoupon.Coupon.DiscountPercent;
					userCouponResponse.DiscountAmount = userCoupon.Coupon.DiscountAmount;
					response.Add(userCouponResponse);
				}

				return new ApiResponse<List<UserCouponResponse>>(response, true, "Retrieve coupons successfully");
			} catch (Exception ex) {
				return new ApiResponse<List<UserCouponResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> RemoveUserCoupon(Guid id) {
			try {
				var userCoupon = await _unitOfWork.UserCoupon.GetAsync(x => x.Id == id);

				if (userCoupon == null) { return false; }

				await _unitOfWork.UserCoupon.RemoveAsync(userCoupon);
				await _unitOfWork.SaveChangeAsync();

				return true;
			} catch (Exception ex) {
				throw new Exception($"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
