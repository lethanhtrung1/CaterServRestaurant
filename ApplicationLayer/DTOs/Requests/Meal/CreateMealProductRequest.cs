namespace ApplicationLayer.DTOs.Requests.Meal {
	public class CreateMealProductRequest {
		public Guid MealId { get; set; }
		public List<Guid> ProductIds { get; set; }
		public int Quantity { get; set; }
	}
}
