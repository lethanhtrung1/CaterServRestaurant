using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Requests.UserProfile;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.UserProfile;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace ApplicationLayer.Services {
	public class UserProfileService : IUserProfileService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper,
			ICurrentUserService currentUserService,
			IHttpContextAccessor httpContextAccessor
		) {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_currentUserService = currentUserService;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<ApiResponse<UserProfileResponse>> GetByUserId(string userId) {
			try {
				var userProfile = await _unitOfWork.UserProfile.GetAsync(x => x.UserId == userId);

				if (userProfile == null) {
					return new ApiResponse<UserProfileResponse>(false, "User profile not found");
				}

				var result = _mapper.Map<UserProfileResponse>(userProfile);

				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (result.Avatar != null) {
					result.Avatar = $"{baseUrl}/{result.Avatar}";
				}

				return new ApiResponse<UserProfileResponse>(result, true, "");
			} catch (Exception ex) {
				return new ApiResponse<UserProfileResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<UserProfileResponse>> UpSertCustomer(CreateOrUpdateUserProfileRequest request) {
			try {
				var currentUserId = _currentUserService.UserId;
				if (currentUserId != request.UserId) {
					return new ApiResponse<UserProfileResponse>(false, "User Id request not match");
				}

				var checkUserProfile = await _unitOfWork.UserProfile.GetAsync(x => x.UserId == request.UserId);
				var result = new UserProfileResponse();

				if (checkUserProfile == null) {
					var newUserProfile = new UserProfile {
						Id = Guid.NewGuid(),
						UserId = request.UserId,
						FirstName = request.FirstName!,
						LastName = request.LastName!,
						Birthday = request.Birthday,
						Gender = request.Gender!,
						PhoneNumber = request.PhoneNumber!,
						Address = request.Address!,
						Bank = "",
						BankBranch = "",
						BankNumber = "",
					};
					var file = request.File;
					var folderName = Path.Combine("Resources", "Images");
					var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
					if (file is not null) {
						var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
						var fullPath = Path.Combine(pathToSave, fileName);
						var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
						using (var stream = new FileStream(fullPath, FileMode.Create)) {
							file.CopyTo(stream);
						}

						newUserProfile.Avatar = dbPath;
					}
					await _unitOfWork.UserProfile.AddAsync(newUserProfile);

					// return response
					result = _mapper.Map<UserProfileResponse>(newUserProfile);
				} else {
					checkUserProfile.UserId = request.UserId;
					checkUserProfile.FirstName = request.FirstName!;
					checkUserProfile.LastName = request.LastName!;
					checkUserProfile.Birthday = request.Birthday;
					checkUserProfile.Gender = request.Gender!;
					checkUserProfile.PhoneNumber = request.PhoneNumber!;
					checkUserProfile.Address = request.Address!;

					if (request.File != null) {
						// Handle remove image
						if (checkUserProfile.Avatar != null) {
							string imagePath = Path.Combine(Directory.GetCurrentDirectory(), checkUserProfile.Avatar);
							if (File.Exists(imagePath)) {
								File.Delete(imagePath);
							}
						}

						var file = request.File;
						var folderName = Path.Combine("Resources", "Images");
						var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

						var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
						var fullPath = Path.Combine(pathToSave, fileName);
						var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
						using (var stream = new FileStream(fullPath, FileMode.Create)) {
							file.CopyTo(stream);
						}

						checkUserProfile.Avatar = dbPath;
					}

					await _unitOfWork.UserProfile.UpdateAsync(checkUserProfile);
					result = _mapper.Map<UserProfileResponse>(checkUserProfile);
				}

				await _unitOfWork.SaveChangeAsync();
				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (result.Avatar != null) {
					result.Avatar = $"{baseUrl}/{result.Avatar}";
				}

				return new ApiResponse<UserProfileResponse>(result, true, "Create or update User Profile success");
			} catch (Exception ex) {
				return new ApiResponse<UserProfileResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<StaffProfileResponse>> UpSertStaff(CreateOrUpdateStaffProfileRequest request) {
			try {
				var currentUserId = _currentUserService.UserId;
				if (currentUserId != request.UserId) {
					return new ApiResponse<StaffProfileResponse>(false, "User Id request not match");
				}

				var checkUserProfile = await _unitOfWork.UserProfile.GetAsync(x => x.UserId == request.UserId);
				var result = new StaffProfileResponse();

				if (checkUserProfile == null) {
					var newUserProfile = new UserProfile {
						Id = Guid.NewGuid(),
						UserId = request.UserId,
						FirstName = request.FirstName!,
						LastName = request.LastName!,
						Birthday = request.Birthday,
						Gender = request.Gender!,
						PhoneNumber = request.PhoneNumber!,
						Address = request.Address!,
						Bank = request.Bank!,
						BankBranch = request.BankNumber!,
						BankNumber = request.BankBranch!,
					};
					var file = request.File;
					var folderName = Path.Combine("Resources", "Images");
					var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
					if (file is not null) {
						var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
						var fullPath = Path.Combine(pathToSave, fileName);
						var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
						using (var stream = new FileStream(fullPath, FileMode.Create)) {
							file.CopyTo(stream);
						}

						newUserProfile.Avatar = dbPath;
					}
					await _unitOfWork.UserProfile.AddAsync(newUserProfile);

					// return response
					result = _mapper.Map<StaffProfileResponse>(newUserProfile);
				} else {
					checkUserProfile.UserId = request.UserId;
					checkUserProfile.FirstName = request.FirstName!;
					checkUserProfile.LastName = request.LastName!;
					checkUserProfile.Birthday = request.Birthday;
					checkUserProfile.Gender = request.Gender!;
					checkUserProfile.PhoneNumber = request.PhoneNumber!;
					checkUserProfile.Address = request.Address!;
					checkUserProfile.Bank = request.Bank!;
					checkUserProfile.BankNumber = request.BankNumber!;
					checkUserProfile.BankBranch = request.BankBranch!;

					if (request.File != null) {
						// Handle remove image
						if (checkUserProfile.Avatar != null) {
							string imagePath = Path.Combine(Directory.GetCurrentDirectory(), checkUserProfile.Avatar);
							if (File.Exists(imagePath)) {
								File.Delete(imagePath);
							}
						}

						var file = request.File;
						var folderName = Path.Combine("Resources", "Images");
						var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

						var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"'); // Content-Disposition
						var fullPath = Path.Combine(pathToSave, fileName);
						var dbPath = Path.Combine(folderName, fileName).Replace("\\", "/");
						using (var stream = new FileStream(fullPath, FileMode.Create)) {
							file.CopyTo(stream);
						}

						checkUserProfile.Avatar = dbPath;
					}

					await _unitOfWork.UserProfile.UpdateAsync(checkUserProfile);
					result = _mapper.Map<StaffProfileResponse>(checkUserProfile);
				}

				await _unitOfWork.SaveChangeAsync();
				var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
				if (result.Avatar != null) {
					result.Avatar = $"{baseUrl}/{result.Avatar}";
				}

				return new ApiResponse<StaffProfileResponse>(result, true, "Create or update User Profile success");
			} catch (Exception ex) {
				return new ApiResponse<StaffProfileResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
