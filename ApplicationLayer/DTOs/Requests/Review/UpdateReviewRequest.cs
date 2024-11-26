namespace ApplicationLayer.DTOs.Requests.Review {
	public class UpdateReviewRequest {
		public Guid Id { get; set; }
		public string? ReviewText { get; set; }
		public DateTime? ReviewDate { get; set; }
		public int? Rating { get; set; }
	}
}
