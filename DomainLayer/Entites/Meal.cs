namespace DomainLayer.Entites {
	public class Meal : BaseEntity {
		public Guid CustomerId { get; set; }
		public Guid TableId { get; set; }
		public decimal TotalPrice { get; set; }

		public ApplicationUser Customer { get; set; }
		public Table Table { get; set; }
		public virtual IEnumerable<MealProduct> MealProducts { get; set; }
	}
}
