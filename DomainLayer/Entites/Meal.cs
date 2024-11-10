namespace DomainLayer.Entites {
	public class Meal : BaseEntity {
		public string CustomerId { get; set; }
		public Guid TableId { get; set; }
		public decimal TotalPrice { get; set; }
		public DateTime CreatedDate { get; set; }

		public ApplicationUser Customer { get; set; }
		public Table Table { get; set; }
		public virtual IEnumerable<MealProduct> MealProducts { get; set; }
	}
}
