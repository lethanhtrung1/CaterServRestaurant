namespace ApplicationLayer.DTOs.Responses.Menu {
	public class MenuResponse {
		public Guid Id { get; set; }
		public string? MenuName { get; set; }
		public string? Description { get; set; }
		public bool Inactive { get; set; }
		public int SortOrder { get; set; }
		public string? ImageUrl { get; set; }
	}
}
