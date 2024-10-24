namespace DomainLayer.Entites {
	public class MealProduct : BaseEntity {
		public Guid MealId { get; set; }
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public Meal Meal { get; set; }
		public Product Product { get; set; }
	}
}
