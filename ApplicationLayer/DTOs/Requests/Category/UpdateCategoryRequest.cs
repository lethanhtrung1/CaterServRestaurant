namespace ApplicationLayer.DTOs.Requests.Category {
	public class UpdateCategoryRequest {
		public Guid Id { get; set; }
		public string? Code { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public bool Inactive { get; set; }
	}
}
