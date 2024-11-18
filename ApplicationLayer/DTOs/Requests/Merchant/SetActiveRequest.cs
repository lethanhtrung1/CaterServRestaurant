namespace ApplicationLayer.DTOs.Requests.Merchant {
	public class SetActiveRequest {
		public Guid Id { get; set; }
		public bool IsActive { get; set; }
	}
}
