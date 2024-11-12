namespace ApplicationLayer.DTOs.Responses.Booking
{
    public class BookingResponse
    {
        public Guid Id { get; set; }
        public int PeopleCount { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckinTime { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
    }
}
