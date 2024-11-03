using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Requests.Menu {
	public class CreateMenuRequest {
		public string? MenuName { get; set; }
		public string? Description { get; set; }
		public bool Inactive { get; set; }
		public int SortOrder { get; set; }
		//public string? ImageUrl { get; set; }
		public IFormFile? File { get; set; }
	}
}
