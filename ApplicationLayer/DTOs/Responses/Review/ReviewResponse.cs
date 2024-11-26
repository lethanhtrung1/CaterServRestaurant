namespace ApplicationLayer.DTOs.Responses.Review {
	public class ReviewResponse {
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string UserName { get; set; }
		public string? ReviewText { get; set; }
		public DateTime? ReviewDate { get; set; }
		public int? Rating { get; set; }
	}
}
