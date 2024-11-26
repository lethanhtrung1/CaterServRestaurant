using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Entites {
	public class Review : BaseEntity {
		public string ReviewText { get; set; }
		public DateTime? ReviewDate { get; set; }
		[Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
		public int? Rating { get; set; }
		public string UserId { get; set; }
		public Guid ProductId { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual Product Product { get; set; }
	}
}
