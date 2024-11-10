namespace ApplicationLayer.DTOs.Requests.Meal {
	public class UpdateMealProductRequest {
		public Guid MealId { get; set; }
		public Guid MealProductId { get; set; }
	}
}
