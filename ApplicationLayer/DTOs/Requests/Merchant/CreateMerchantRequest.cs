namespace ApplicationLayer.DTOs.Requests.Merchant {
	public class CreateMerchantRequest {
		public string? MerchantName { get; set; }
		public string? MerchantWebLink { get; set; }
		public string? MerchantIpnUrl { get; set; }
		public string? MerchantReturnUrl { get; set; }
	}
}
