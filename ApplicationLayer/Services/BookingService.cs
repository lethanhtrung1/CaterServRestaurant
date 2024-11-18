using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Booking;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Booking;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.AspNetCore.Identity;

namespace ApplicationLayer.Services
{
    public class BookingService : IBookingService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogException _logger;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly UserManager<ApplicationUser> _userManager;

		public BookingService(IUnitOfWork unitOfWork, ILogException logger, IMapper mapper,
			ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_currentUserService = currentUserService;
			_userManager = userManager;
		}

		public async Task<ApiResponse<BookingResponse>> CreateAsync(CreateBookingRequest request) {
			try {
				var bookingToDb = _mapper.Map<Booking>(request);
				var customerId = _currentUserService.UserId;
				if (!string.IsNullOrEmpty(customerId)) {
					bookingToDb.CustomerId = customerId;
				}
				await _unitOfWork.Booking.AddAsync(bookingToDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<BookingResponse>(bookingToDb);
				return new ApiResponse<BookingResponse>(result, true, "Created successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<BookingResponse>> CreateForStaffAsync(CreateBookingRequest request) {
			try {
				var bookingToDb = _mapper.Map<Booking>(request);
				var customer = await _userManager.FindByNameAsync(request.CustomerName!);
				if (!string.IsNullOrEmpty(customer?.Id)) {
					bookingToDb.CustomerId = customer?.Id;
				}
				await _unitOfWork.Booking.AddAsync(bookingToDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<BookingResponse>(bookingToDb);
				return new ApiResponse<BookingResponse>(result, true, "Created successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> DeleteAsync(Guid id) {
			try {
				var booking = await _unitOfWork.Booking.GetAsync(x => x.Id == id);
				if (booking == null) {
					return false;
				}

				await _unitOfWork.Booking.RemoveAsync(booking);
				await _unitOfWork.SaveChangeAsync();

				return true;
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				throw new Exception(ex.Message);
			}
		}

		public async Task<ApiResponse<BookingResponse>> GetAsync(Guid id) {
			try {
				var booking = await _unitOfWork.Booking.GetAsync(x => x.Id == id);

				if (booking is null) {
					return new ApiResponse<BookingResponse>(false, $"Booking with Id: {id} not exist");
				}

				var result = _mapper.Map<BookingResponse>(booking);
				return new ApiResponse<BookingResponse>(result, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<BookingResponse>>> GetListAsync(PagingRequest request) {
			try {
				var bookings = await _unitOfWork.Booking.GetListAsync();
				var bookingPagedList = bookings.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (bookingPagedList is null || !bookingPagedList.Any()) {
					return new ApiResponse<PagedList<BookingResponse>>(false, "No record available");
				}

				int totalRecord = bookings.Count();
				var result = _mapper.Map<List<BookingResponse>>(bookingPagedList);

				return new ApiResponse<PagedList<BookingResponse>>(
					new PagedList<BookingResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, ""
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<BookingResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<BookingResponse>> UpdateAsync(UpdateBookingRequest request) {
			try {
				var bookingFromDb = await _unitOfWork.Booking.GetAsync(x => x.Id == request.Id);

				if (bookingFromDb is null) {
					return new ApiResponse<BookingResponse>(false, $"Booking with Id: {request.Id} not exist");
				}

				_mapper.Map(request, bookingFromDb);
				await _unitOfWork.Booking.UpdateAsync(bookingFromDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<BookingResponse>(bookingFromDb);
				return new ApiResponse<BookingResponse>(result, true, "Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
