namespace DomainLayer.Entites {
	public class BookingTable : BaseEntity {
		public Guid BookingId { get; set; }
		public Guid TableId { get; set; }
		public Booking Booking { get; set; }
		public Table Table { get; set; }
	}
}
