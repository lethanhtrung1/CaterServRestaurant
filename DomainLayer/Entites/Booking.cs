namespace DomainLayer.Entites {
	public class Booking : BaseEntity {
		public int PeopleCount { get; set; }
		public string Status { get; set; }
		public string? Notes { get; set; }
		public DateTime? BookingDate { get; set; }
		public DateTime? CheckinTime { get; set; }
		public string? CustomerId { get; set; }
		public string CustomerName { get; set; }
		public string Phone {  get; set; }

		public ApplicationUser? Customer { get; set; }
		public virtual IEnumerable<BookingTable> BookingTables { get; set; }
		public virtual IEnumerable<Order> Orders { get; set; }
	}
}
