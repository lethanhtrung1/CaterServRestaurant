using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Booking;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Booking;

namespace ApplicationLayer.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponse<BookingResponse>> GetAsync(Guid id);
        Task<ApiResponse<PagedList<BookingResponse>>> GetListAsync(PagingRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<ApiResponse<BookingResponse>> CreateAsync(CreateBookingRequest request);
        Task<ApiResponse<BookingResponse>> UpdateAsync(UpdateBookingRequest request);
    }
}
