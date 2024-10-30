namespace ApplicationLayer.DTOs.Responses.Category {
	public class CategoryResponseDto {
		public Guid Id { get; set; }
		public string? Code { get; set; }
		public string? Name { get; set; }
		public string? Description { get; set; }
		public bool Inactive { get; set; }
	}
}
