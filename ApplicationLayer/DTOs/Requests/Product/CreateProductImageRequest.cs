using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Requests.Product {
	public class CreateProductImageRequest {
		public Guid ProductId { get; set; }
		public List<IFormFile> Files { get; set; }
	}
}
