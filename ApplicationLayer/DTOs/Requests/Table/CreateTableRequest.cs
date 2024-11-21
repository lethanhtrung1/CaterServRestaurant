namespace ApplicationLayer.DTOs.Requests.Table {
	public class CreateTableRequest {
		public string? Name { get; set; }
		public int MaxCapacity { get; set; }
		//public bool IsAvailable { get; set; }
		//public Guid AreaId { get; set; }
		public string? AreaName { get; set; }
	}
}
