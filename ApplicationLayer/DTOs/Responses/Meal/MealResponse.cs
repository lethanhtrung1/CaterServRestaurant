namespace ApplicationLayer.DTOs.Responses.Meal {
	public class MealResponse {
		public Guid Id { get; set; }
		public decimal TotalPrice { get; set; }
		public List<MealProductResponse>? Products { get; set; }
		public Guid TableId { get; set; }
		public string? TableName { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
