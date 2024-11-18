namespace DomainLayer.Entites {
	public class Merchant : BaseEntity {
		public string? MerchantName { get; set; }
		public string? MerchantWebLink { get; set; }
		public string? MerchantIpnUrl { get; set; }
		public string? MerchantReturnUrl { get; set; }
		public string? SecretKey { get; set; }
		public bool IsActive { get; set; }

		public virtual IEnumerable<Payment>? Payments { get; set; }
	}
}
