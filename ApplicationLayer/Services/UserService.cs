using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.User;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.User;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

				user.LockoutEnd = DateTimeOffset.MaxValue;
				//user.LockoutEnd = DateTime.MaxValue;

				await _unitOfWork.ApplicationUser.UpdateAsync(user);
				await _unitOfWork.SaveChangeAsync();
				return true;
			} catch (Exception ex) {
				throw new Exception("Error occured while ban user", ex);
			}
		}

		public async Task<ApiResponse<UserResponse>> ChangeRole(string userId, string roleName) {
			var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);

			if (user == null) {
				return new ApiResponse<UserResponse>(false, "user not found");
			}

			var oleRole = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!;
			if (oleRole != roleName) {
				await _userManager.RemoveFromRoleAsync(user, oleRole);
				await _userManager.AddToRoleAsync(user, roleName);
			}

			return new ApiResponse<UserResponse>(true, "Change Role successfully");
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

				await _unitOfWork.UserProfile.BeginTransactionAsync();

				var createUser = await _userManager.CreateAsync(newUser, request.Password);
				if (!createUser.Succeeded) {
					await _unitOfWork.UserProfile.RollBackTransactionAsync();
					return new ApiResponse<UserResponse>(false, "Error occured while creating account");
				}

				var role = await _roleManager.FindByNameAsync(request.RoleName);

				if (role == null) {
					await _unitOfWork.UserProfile.RollBackTransactionAsync();
					return new ApiResponse<UserResponse>(false, "Error occured while creating account");
				}

				// Add role for user
				IdentityResult assignRoleResult = await _userManager.AddToRoleAsync(newUser, role.Name!);
				if (!assignRoleResult.Succeeded) {
					await _unitOfWork.UserProfile.RollBackTransactionAsync();
					return new ApiResponse<UserResponse>(false, "Error occured while creating account");
				}

				var userProfile = new UserProfile {
					Id = Guid.NewGuid(),
					FirstName = request.FirstName!,
					LastName = request.LastName!,
					PhoneNumber = request.PhoneNumber!,
					Address = request.Address!,
					Gender = request.Gender!,
					Birthday = request.Birthday,
					Bank = request.Bank,
					BankBranch = request.BankBranch,
					BankNumber = request.BankNumber,
				};

				await _unitOfWork.UserProfile.AddAsync(userProfile);
				await _unitOfWork.SaveChangeAsync();
				await _unitOfWork.UserProfile.EndTransactionAsync();

				return new ApiResponse<UserResponse>(true, "Create new user successfully");
			} catch (Exception ex) {
				await _unitOfWork.UserProfile.RollBackTransactionAsync();
				throw new Exception("Error occured while creating account", ex);
			}
		}

		public async Task<ApiResponse<UserResponse>> GetById(string id) {
			try {
				var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == id);

				if (user == null) {
					return new ApiResponse<UserResponse>(false, $"User with id: {id} not found");
				}

				var role = await _userManager.GetRolesAsync(user);

				var result = _mapper.Map<UserResponse>(user);
				result.RoleName = role.FirstOrDefault()!;

				return new ApiResponse<UserResponse>(result, true, "Retrieve user successfully");
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
				//var result = _mapper.Map<List<UserResponse>>(usersPagedList);
				var result = new List<UserResponse>();
				foreach (var user in users) {
					var role = await _userManager.GetRolesAsync(user);
					var userResponse = _mapper.Map<UserResponse>(user);
					userResponse.UserName = role.FirstOrDefault()!;
					result.Add(userResponse);
				}

				return new ApiResponse<PagedList<UserResponse>>(
					new PagedList<UserResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, "Retrieve users successfully"
				);
			} catch (Exception ex) {
				throw new Exception("Error occured while retrieving users", ex);
			}
		}

		public async Task<ApiResponse<List<RoleResponse>>> GetRoles() {
			var roles = await _roleManager.Roles.ToListAsync();
			var result = _mapper.Map<List<RoleResponse>>(roles);
			return new ApiResponse<List<RoleResponse>>(result, true, "Retrieve Roles successfully");
		}

		public async Task<bool> UnbanUser(string id) {
			try {
				var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == id);
				if (user == null) return false;

				user.LockoutEnd = null;

				await _unitOfWork.ApplicationUser.UpdateAsync(user);
				await _unitOfWork.SaveChangeAsync();
				return true;
			} catch (Exception ex) {
				throw new Exception("Error occured while unban user", ex);
			}
		}
	}
}
