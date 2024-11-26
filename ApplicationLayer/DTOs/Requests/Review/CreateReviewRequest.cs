namespace ApplicationLayer.DTOs.Requests.Review {
	public class CreateReviewRequest {
		public string? ReviewText { get; set; } = string.Empty;
		//public DateTime? ReviewDate { get; set; } = DateTime.Now;
		public int? Rating { get; set; }
		public Guid ProductId { get; set; }
	}
}
