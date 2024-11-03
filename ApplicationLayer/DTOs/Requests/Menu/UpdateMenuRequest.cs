using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Requests.Menu {
	public class UpdateMenuRequest {
		public Guid Id { get; set; }
		public string? MenuName { get; set; }
		public string? Description { get; set; }
		public bool Inactive { get; set; }
		public int SortOrder { get; set; }
		//public string? ImageUrl { get; set; }
		public IFormFile? File { get; set; }
	}
}
