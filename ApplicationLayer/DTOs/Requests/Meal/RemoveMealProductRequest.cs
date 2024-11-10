namespace ApplicationLayer.DTOs.Requests.Meal {
	public class RemoveMealProductRequest {
		public Guid MealId { get; set; }
		public Guid MealProductId { get; set; }
	}
}
