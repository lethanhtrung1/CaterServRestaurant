namespace ApplicationLayer.DTOs.Responses.Meal {
	public class MealProductResponse {
		public Guid Id { get; set; }
		public MealProductDetailDto ProductDetail { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}
