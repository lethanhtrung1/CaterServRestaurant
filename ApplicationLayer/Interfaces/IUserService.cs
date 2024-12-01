using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.User;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.User;

namespace ApplicationLayer.Interfaces {
	public interface IUserService {
		Task<ApiResponse<UserResponse>> GetById(string id);
		Task<ApiResponse<PagedList<UserResponse>>> GetPaging(PagingRequest request);
		Task<ApiResponse<UserResponse>> CreateUser(CreateUserRequest request);
		Task<bool> BanUser(string id);
		Task<bool> UnbanUser(string id);
		Task<ApiResponse<List<RoleResponse>>> GetRoles();
		Task<ApiResponse<UserResponse>> ChangeRole(string userId, string roleId); 
	}
}
