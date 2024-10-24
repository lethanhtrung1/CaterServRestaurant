namespace DomainLayer.Entites {
	public class Table : BaseEntity {
		public string Name { get; set; }
		public int MaxCapacity { get; set; }
		public bool IsAvailable { get; set; }
		public Guid AreaId { get; set; }
		public string AreaName { get; set; }
		public virtual IEnumerable<BookingTable> BookingTables { get; set; }
	}
}
