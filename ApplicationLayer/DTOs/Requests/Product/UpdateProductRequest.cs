using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Requests.Product {
	public class UpdateProductRequest {
		public Guid Id { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public double Price { get; set; }
		public double SellingPrice { get; set; }
		public string? UnitName { get; set; }
		public IFormFile? File { get; set; }
		public bool Inactive { get; set; }
		public Guid CategoryId { get; set; }
		public Guid MenuId { get; set; }
	}
}
