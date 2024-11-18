namespace ApplicationLayer.DTOs.Responses.Merchant {
	public class MerchantResponse {
		public Guid Id { get; set; }
		public string? MerchantName { get; set; }
		public string? MerchantWebLink { get; set; }
		public string? MerchantIpnUrl { get; set; }
		public string? MerchantReturnUrl { get; set; }
		public string? SecretKey { get; set; }
		public bool IsActive { get; set; }
	}
}
