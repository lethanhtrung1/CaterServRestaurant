using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Coupon;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Coupon;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Caching;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class CouponService : ICouponService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly ICacheService _cacheService;

		public CouponService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper, ICacheService cacheService) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_cacheService = cacheService;
		}

		public async Task<ApiResponse<CouponResponse>> CreateAsync(CreateCouponRequest request) {
			try {
				var couponToDb = _mapper.Map<Coupon>(request);

				await _unitOfWork.Coupon.AddAsync(couponToDb);
				await _unitOfWork.SaveChangeAsync();

				// Set initial quantity into Redis
				var couponKey = $"coupon:{couponToDb.Id}:quantity";
				await _cacheService.SetData(couponKey, couponToDb.Quantity, DateTimeOffset.UtcNow.AddDays(7));

				// Map response and return
				var response = _mapper.Map<CouponResponse>(couponToDb);
				return new ApiResponse<CouponResponse>(response, true, "Created successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CouponResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			var coupon = await _unitOfWork.Coupon.GetAsync(x => x.Id == id);

			if (coupon == null) {
				return false;
			}

			await _unitOfWork.Coupon.RemoveAsync(coupon);
			await _unitOfWork.SaveChangeAsync();

			// Remove cache
			var couponKey = $"coupon:{id}:quantity";
			await _cacheService.RemoveData(couponKey);

			return true;
		}

		public async Task<ApiResponse<CouponResponse>> GetByCodeAsync(string code) {
			try {
				var coupon = await _unitOfWork.Coupon.GetAsync(x => x.CouponCode == code);

				if (coupon == null) {
					return new ApiResponse<CouponResponse>(false, $"Coupon with code: {code} not found");
				}

				var couponDto = _mapper.Map<CouponResponse>(coupon);

				if (couponDto == null) {
					return new ApiResponse<CouponResponse>(false, "Internal server error occurred");
				}

				return new ApiResponse<CouponResponse>(couponDto, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CouponResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CouponResponse>> GetByIdAsync(Guid id) {
			try {
				var coupon = await _unitOfWork.Coupon.GetAsync(x => x.Id == id);

				if (coupon == null) {
					return new ApiResponse<CouponResponse>(false, $"Coupon with id: {id} not found");
				}

				var couponDto = _mapper.Map<CouponResponse>(coupon);

				if (couponDto == null) {
					return new ApiResponse<CouponResponse>(false, "Internal server error occurred");
				}

				return new ApiResponse<CouponResponse>(couponDto, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CouponResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CouponResponse>> GetCouponActiveByIdAsync(Guid id) {
			try {
				var coupon = await _unitOfWork.Coupon.GetAsync(x => x.Id == id && !x.Inactive);

				if (coupon == null) {
					return new ApiResponse<CouponResponse>(false, $"Coupon with id: {id} not found");
				}

				var couponDto = _mapper.Map<CouponResponse>(coupon);

				if (couponDto == null) {
					return new ApiResponse<CouponResponse>(false, "Internal server error occurred");
				}

				return new ApiResponse<CouponResponse>(couponDto, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CouponResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<CouponResponse>>> GetListAsync(PagingRequest request) {
			try {
				var coupons = await _unitOfWork.Coupon.GetListAsync();
				var couponsPagedList = coupons.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (!couponsPagedList.Any()) {
					new ApiResponse<PagedList<CouponResponse>>(false, "No record available");
				}

				int totalRecord = coupons.Count();
				var result = _mapper.Map<List<CouponResponse>>(couponsPagedList);

				return new ApiResponse<PagedList<CouponResponse>>(
					new PagedList<CouponResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<CouponResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<CouponResponse>>> GetListCouponActiveAsync(PagingRequest request) {
			try {
				var coupons = await _unitOfWork.Coupon.GetListAsync(x => !x.Inactive);
				var couponsPagedList = coupons.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (!couponsPagedList.Any()) {
					new ApiResponse<PagedList<CouponResponse>>(false, "No record available");
				}

				int totalRecord = coupons.Count();
				var result = _mapper.Map<List<CouponResponse>>(couponsPagedList);

				return new ApiResponse<PagedList<CouponResponse>>(
					new PagedList<CouponResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<CouponResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<CouponResponse>> UpdateAsync(UpdateCouponRequest request) {
			try {
				var checkCouponFromDb = await _unitOfWork.Coupon.GetAsync(x => x.Id == request.Id);

				if (checkCouponFromDb == null) {
					return new ApiResponse<CouponResponse>(false, $"Coupon with id: {request.Id} not found");
				}

				// Update coupon
				if (!string.IsNullOrEmpty(request.CouponCode) && checkCouponFromDb.CouponCode != request.CouponCode) {
					checkCouponFromDb.CouponCode = request.CouponCode;
				}
				checkCouponFromDb.DiscountPercent = request.DiscountPercent;
				checkCouponFromDb.DiscountAmount = request.DiscountAmount;
				checkCouponFromDb.Quantity = request.Quantity;
				checkCouponFromDb.Inactive = request.Inactive;

				await _unitOfWork.Coupon.UpdateAsync(checkCouponFromDb);
				await _unitOfWork.SaveChangeAsync();

				// Update cache
				try {
					var couponKey = $"coupon:{request.Id}:quantity";
					await _cacheService.SetData(couponKey, checkCouponFromDb.Quantity, DateTimeOffset.UtcNow.AddDays(7));
				} catch (Exception cacheEx) {
					_logger.LogToDebugger($"Failed to update cache for coupon {request.Id}: {cacheEx.Message}");
				}

				// Map response
				var response = _mapper.Map<CouponResponse>(checkCouponFromDb);
				return new ApiResponse<CouponResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<CouponResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
