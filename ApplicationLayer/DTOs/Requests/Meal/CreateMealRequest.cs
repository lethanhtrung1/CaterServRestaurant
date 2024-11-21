namespace ApplicationLayer.DTOs.Requests.Meal {
	public class CreateMealRequest {
		//public Guid TableId { get; set; }
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
	}
}
