namespace ApplicationLayer.DTOs.Requests.Table {
	public class UpdateTableStatus {
		public Guid TableId { get; set; }
		public string Status { get; set; }
	}
}
