using ApplicationLayer.DTOs.Requests.UserProfile;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.UserProfile;

namespace ApplicationLayer.Interfaces {
	public interface IUserProfileService {
		Task<ApiResponse<UserProfileResponse>> GetByUserId(string userId);
		Task<ApiResponse<UserProfileResponse>> UpSertCustomer(CreateOrUpdateUserProfileRequest request);
		Task<ApiResponse<StaffProfileResponse>> UpSertStaff(CreateOrUpdateStaffProfileRequest request);
	}
}
