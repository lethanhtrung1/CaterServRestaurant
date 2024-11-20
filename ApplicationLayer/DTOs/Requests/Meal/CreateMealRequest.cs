namespace ApplicationLayer.DTOs.Requests.Meal {
	public class CreateMealRequest {
		public Guid TableId { get; set; }
		public List<MealProductDto> Products { get; set; }
	}
}
