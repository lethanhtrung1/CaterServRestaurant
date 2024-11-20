namespace ApplicationLayer.DTOs.Requests.Meal {
	public class CreateMealProductRequest {
		public Guid MealId { get; set; }
		public List<MealProductDto> Products { get; set; }
	}
}
