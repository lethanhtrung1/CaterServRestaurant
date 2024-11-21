using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.User;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.User;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Identity;

namespace ApplicationLayer.Services {
	public class UserService : IUserService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<UserRole> _roleManager;
		private readonly IMapper _mapper;

		public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<UserRole> roleManager, IMapper mapper) {
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
		}

		public async Task<bool> BanUser(string id) {
			try {
				var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == id);
				if (user == null) return false;
				user.LockoutEnabled = true;
				await _unitOfWork.ApplicationUser.UpdateAsync(user);
				await _unitOfWork.SaveChangeAsync();
				return true;
			} catch (Exception ex) {
				throw new Exception("Error occured while ban user", ex);
			}
		}

		public async Task<ApiResponse<UserResponse>> CreateUser(CreateUserRequest request) {
			try {
				var checkUserExist = await _userManager.FindByEmailAsync(request.Email);
				if (checkUserExist is not null) {
					return new ApiResponse<UserResponse>(false, "Email already exist");
				}

				var newUser = new ApplicationUser {
					Name = request.Email,
					Email = request.Email,
					UserName = request.Email,
					PasswordHash = request.Password
				};

				var createUser = await _userManager.CreateAsync(newUser, request.Password);
				if (!createUser.Succeeded) {
					return new ApiResponse<UserResponse>(false, "Error occured while creating account");
				}

				// Add role for user
				IdentityResult assignRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role);
				if (!assignRoleResult.Succeeded) {
					return new ApiResponse<UserResponse>(false, "Error occured while creating account");
				}

				return new ApiResponse<UserResponse>(true, "Create new user successfully");
			} catch (Exception ex) {
				throw new Exception("Error occured while creating account", ex);
			}
		}

		public async Task<ApiResponse<UserResponse>> GetById(string id) {
			try {
				var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == id);

				if (user == null) {
					return new ApiResponse<UserResponse>(false, $"User with id: {id} not found");
				}

				var result = _mapper.Map<UserResponse>(user);

				return new ApiResponse<UserResponse>(result, true, "");
			} catch (Exception ex) {
				throw new Exception("Error occured while retrieving user", ex);
			}
		}

		public async Task<ApiResponse<PagedList<UserResponse>>> GetPaging(PagingRequest request) {
			try {
				var users = await _unitOfWork.ApplicationUser.GetListAsync();

				var usersPagedList = users.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

				if (usersPagedList.Count == 0) {
					return new ApiResponse<PagedList<UserResponse>>(false, "No record available");
				}

				int totalRecord = users.Count();
				var result = _mapper.Map<List<UserResponse>>(usersPagedList);

				return new ApiResponse<PagedList<UserResponse>>(
					new PagedList<UserResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				throw new Exception("Error occured while retrieving users", ex);
			}
		}

		public async Task<bool> UnbanUser(string id) {
			try {
				var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == id);
				if (user == null) return false;
				user.LockoutEnabled = false;
				await _unitOfWork.ApplicationUser.UpdateAsync(user);
				await _unitOfWork.SaveChangeAsync();
				return true;
			} catch (Exception ex) {
				throw new Exception("Error occured while unban user", ex);
			}
		}
	}
}
