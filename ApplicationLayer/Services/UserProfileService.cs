using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Requests.UserProfile;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.UserProfile;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using ApplicationLayer.Options;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.Extensions.Options;

namespace ApplicationLayer.Services {
	public class UserProfileService : IUserProfileService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly Cloudinary _cloudinary;
		private readonly CloudinaryOptions _cloudinaryOptions;
		private readonly ILogException _logger;

		public UserProfileService(IUnitOfWork unitOfWork, IMapper mapper,
			ICurrentUserService currentUserService,
			IOptions<CloudinaryOptions> options,
			ILogException logger
		) {
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_currentUserService = currentUserService;
			_logger = logger;
			_cloudinaryOptions = options.Value;
			Account account = new Account(
				_cloudinaryOptions.CloudName,
				_cloudinaryOptions.ApiKey,
				_cloudinaryOptions.ApiSecret
			);
			_cloudinary = new Cloudinary(account);
		}

		public async Task<ApiResponse<UserProfileResponse>> GetByUserId(string userId) {
			try {
				var userProfile = await _unitOfWork.UserProfile.GetAsync(x => x.UserId == userId);

				if (userProfile == null) {
					return new ApiResponse<UserProfileResponse>(false, "User profile not found");
				}

				var result = _mapper.Map<UserProfileResponse>(userProfile);

				return new ApiResponse<UserProfileResponse>(result, true, "Retrieve user profile successfully");
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

				// Create new User profile
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
					// Handle upload image
					var file = request.File;
					if (file is not null) {
						var uploadResult = new ImageUploadResult();
						using (var stream = file.OpenReadStream()) {
							var uploadParam = new ImageUploadParams() {
								File = new FileDescription(file.Name, stream),
								Folder = $"{_cloudinaryOptions.Folder}/User"
							};
							uploadResult = await _cloudinary.UploadAsync(uploadParam);
						}

						newUserProfile.Avatar = uploadResult.Url.ToString();
						newUserProfile.AvatarPublicId = uploadResult.PublicId;
					}
					await _unitOfWork.UserProfile.AddAsync(newUserProfile);
					result = _mapper.Map<UserProfileResponse>(newUserProfile);
				} 
				// Update user profile
				else {
					checkUserProfile.UserId = request.UserId;
					checkUserProfile.FirstName = request.FirstName!;
					checkUserProfile.LastName = request.LastName!;
					checkUserProfile.Birthday = request.Birthday;
					checkUserProfile.Gender = request.Gender!;
					checkUserProfile.PhoneNumber = request.PhoneNumber!;
					checkUserProfile.Address = request.Address!;

					if (request.File != null) {
						// Handle remove image fro cloudinary
						var deletionParam = new DeletionParams(checkUserProfile.AvatarPublicId) {
							ResourceType = ResourceType.Image,
						};
						var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
						if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
							_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {checkUserProfile.AvatarPublicId}");
						}

						// Upload file
						var file = request.File;
						if (file is not null) {
							var uploadResult = new ImageUploadResult();
							using (var stream = file.OpenReadStream()) {
								var uploadParam = new ImageUploadParams() {
									File = new FileDescription(file.Name, stream),
									Folder = $"{_cloudinaryOptions.Folder}/User"
								};
								uploadResult = await _cloudinary.UploadAsync(uploadParam);
							}
							checkUserProfile.Avatar = uploadResult.Url.ToString();
							checkUserProfile.AvatarPublicId = uploadResult.PublicId;
						}
					}

					await _unitOfWork.UserProfile.UpdateAsync(checkUserProfile);
					result = _mapper.Map<UserProfileResponse>(checkUserProfile);
				}
				await _unitOfWork.SaveChangeAsync();

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

				// Create
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
					if (file is not null) {
						var uploadResult = new ImageUploadResult();
						using (var stream = file.OpenReadStream()) {
							var uploadParam = new ImageUploadParams() {
								File = new FileDescription(file.Name, stream),
								Folder = $"{_cloudinaryOptions.Folder}/User"
							};
							uploadResult = await _cloudinary.UploadAsync(uploadParam);
						}

						newUserProfile.Avatar = uploadResult.Url.ToString();
						newUserProfile.AvatarPublicId = uploadResult.PublicId;
					}
					await _unitOfWork.UserProfile.AddAsync(newUserProfile);
					result = _mapper.Map<StaffProfileResponse>(newUserProfile);
				} 
				// Update
				else {
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
						// Handle remove image fro cloudinary
						var deletionParam = new DeletionParams(checkUserProfile.AvatarPublicId) {
							ResourceType = ResourceType.Image,
						};
						var deletionResult = await _cloudinary.DestroyAsync(deletionParam);
						if (deletionResult == null || deletionResult.Result != "ok") { // Delete failed
							_logger.LogExceptions($"Failed to delete image from Cloudinary with PublicId: {checkUserProfile.AvatarPublicId}");
						}

						// Upload file
						var file = request.File;
						if (file is not null) {
							var uploadResult = new ImageUploadResult();
							using (var stream = file.OpenReadStream()) {
								var uploadParam = new ImageUploadParams() {
									File = new FileDescription(file.Name, stream),
									Folder = $"{_cloudinaryOptions.Folder}/User"
								};
								uploadResult = await _cloudinary.UploadAsync(uploadParam);
							}
							checkUserProfile.Avatar = uploadResult.Url.ToString();
							checkUserProfile.AvatarPublicId = uploadResult.PublicId;
						}
					}

					await _unitOfWork.UserProfile.UpdateAsync(checkUserProfile);
					result = _mapper.Map<StaffProfileResponse>(checkUserProfile);
				}

				await _unitOfWork.SaveChangeAsync();

				return new ApiResponse<StaffProfileResponse>(result, true, "Create or update User Profile success");
			} catch (Exception ex) {
				return new ApiResponse<StaffProfileResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
