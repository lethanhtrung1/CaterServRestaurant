namespace ApplicationLayer.DTOs.Responses.Meal {
	public class MealProductDetailDto {
		public Guid Id { get; set; }
		public string? Code { get; set; }
		public string? Name { get; set; }
		public double Price { get; set; }
		public double SellingPrice { get; set; }
		public string? UnitName { get; set; }
		public string? Thumbnail { get; set; }
	}
}
