using ApplicationLayer.Common.Constants;
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

namespace ApplicationLayer.Services {
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

		public async Task<ApiResponse<BookingResponse>> CancelBookingAsync(Guid id) {
			try {
				var bookingFromDb = await _unitOfWork.Booking.GetAsync(x => x.Id == id);

				if (bookingFromDb is null) {
					return new ApiResponse<BookingResponse>(false, $"Booking with Id: {id} not exist");
				}

				bookingFromDb.Status = BookingStatus.Cancel;
				await _unitOfWork.Booking.UpdateAsync(bookingFromDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<BookingResponse>(bookingFromDb);
				return new ApiResponse<BookingResponse>(result, true, "Cancel booking successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<BookingResponse>> ChangeTableAsync(ChangeTableRequest request) {
			try {
				var booking = await _unitOfWork.Booking.GetAsync(x => x.Id == request.BookingId);

				if (booking == null) {
					return new ApiResponse<BookingResponse>(false, $"Booking with id: {request.BookingId} not found");
				}

				var bookingTables = await _unitOfWork.BookingTable.GetListAsync(x => x.BookingId == request.BookingId);
				await _unitOfWork.BookingTable.RemoveRangeAsync(bookingTables);
				foreach (var item in request.TableIds) {
					var newBookingTable = new BookingTable {
						Id = Guid.NewGuid(),
						TableId = item,
						BookingId = request.BookingId,
					};
					await _unitOfWork.BookingTable.AddAsync(newBookingTable);
				}

				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<BookingResponse>(booking);
				return new ApiResponse<BookingResponse>(result, true, "Change table successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<BookingResponse>> CreateAsync(CreateBookingRequest request) {
			try {
				var bookingToDb = _mapper.Map<Booking>(request);
				bookingToDb.BookingDate = DateTime.Now;
				bookingToDb.Status = BookingStatus.Pending;
				var customerId = _currentUserService.UserId;
				if (!string.IsNullOrEmpty(customerId)) {
					bookingToDb.CustomerId = customerId;
				}
				await _unitOfWork.Booking.AddAsync(bookingToDb);
				foreach (var item in request.TableIds) {
					var bookingTable = new BookingTable {
						Id = Guid.NewGuid(),
						TableId = item,
						BookingId = bookingToDb.Id
					};
					await _unitOfWork.BookingTable.AddAsync(bookingTable);
				}
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
				bookingToDb.BookingDate = DateTime.Now;
				bookingToDb.Status = BookingStatus.Accept;

				var customer = await _unitOfWork.ApplicationUser.GetAsync(x => x.UserName == request.CustomerName);
				if (!string.IsNullOrEmpty(customer?.Id)) {
					bookingToDb.CustomerId = customer?.Id;
				}
				await _unitOfWork.Booking.AddAsync(bookingToDb);

				if (request.TableIds != null) {
					foreach (var item in request.TableIds) {
						var bookingTable = new BookingTable {
							Id = Guid.NewGuid(),
							TableId = item,
							BookingId = bookingToDb.Id
						};
						await _unitOfWork.BookingTable.AddAsync(bookingTable);
					}
				}
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

				var bookingTables = await _unitOfWork.BookingTable.GetListAsync(x => x.BookingId == booking.Id);
				await _unitOfWork.Booking.BeginTransactionAsync();

				await _unitOfWork.BookingTable.RemoveRangeAsync(bookingTables);
				await _unitOfWork.Booking.RemoveAsync(booking);

				await _unitOfWork.SaveChangeAsync();
				await _unitOfWork.Booking.EndTransactionAsync();

				return true;
			} catch (Exception ex) {
				await _unitOfWork.Booking.RollBackTransactionAsync();
				_logger.LogExceptions(ex);
				throw new Exception(ex.Message);
			}
		}

		public async Task<ApiResponse<BookingResponse>> GetAsync(Guid id) {
			try {
				var booking = await _unitOfWork.Booking.GetAsync(x => x.Id == id, includeProperties: "BookingTables");

				if (booking is null) {
					return new ApiResponse<BookingResponse>(false, $"Booking with Id: {id} not exist");
				}

				var result = _mapper.Map<BookingResponse>(booking);
				result.Tables = new List<BookingTableResponse>();

				foreach (var bookingTable in booking.BookingTables) {
					var table = await _unitOfWork.Table.GetAsync(x => x.Id == bookingTable.TableId);
					var bookingTableRes = new BookingTableResponse {
						TableId = bookingTable.TableId,
						Name = table.Name,
						AreaName = table.AreaName,
					};
					result.Tables.Add(bookingTableRes);
				}

				return new ApiResponse<BookingResponse>(result, true, "Retrieve booking successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<BookingResponse>>> GetListAsync(PagingRequest request) {
			try {
				var bookings = await _unitOfWork.Booking.GetListAsync(includeProperties: "BookingTables");
				var bookingPagedList = bookings.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

				if (bookingPagedList is null || !bookingPagedList.Any()) {
					return new ApiResponse<PagedList<BookingResponse>>(false, "No record available");
				}


				int totalRecord = bookings.Count();
				//var result = _mapper.Map<List<BookingResponse>>(bookingPagedList);
				var result = new List<BookingResponse>();
				foreach (var booking in bookingPagedList) {
					var bookingResponse = _mapper.Map<BookingResponse>(booking);
					bookingResponse.Tables = new List<BookingTableResponse>();

					foreach (var bookingTable in booking.BookingTables) {
						var table = await _unitOfWork.Table.GetAsync(x => x.Id == bookingTable.TableId);
						var bookingTableRes = new BookingTableResponse {
							TableId = bookingTable.TableId,
							Name = table.Name,
							AreaName = table.AreaName,
						};
						bookingResponse.Tables.Add(bookingTableRes);
					}
					result.Add(bookingResponse);
				}

				return new ApiResponse<PagedList<BookingResponse>>(
					new PagedList<BookingResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, "Retrieve bookings successfully"
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

		public async Task<ApiResponse<BookingResponse>> UpdateStatusAsync(UpdateStatusBookingRequest request) {
			try {
				var bookingFromDb = await _unitOfWork.Booking.GetAsync(x => x.Id == request.BookingId);

				if (bookingFromDb is null) {
					return new ApiResponse<BookingResponse>(false, $"Booking with Id: {request.BookingId} not exist");
				}

				bookingFromDb.Status = request.Status;
				await _unitOfWork.Booking.UpdateAsync(bookingFromDb);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<BookingResponse>(bookingFromDb);
				return new ApiResponse<BookingResponse>(result, true, "Status Updated successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<BookingResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
