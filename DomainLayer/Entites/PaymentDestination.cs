namespace DomainLayer.Entites {
	public class PaymentDestination : BaseEntity {
		public string? DesName { get; set; } 
		public string? DesShortName { get; set; }
		public string? DesLogo { get; set; }
		public int DesSortIndex { get; set; }
		public bool IsActive { get; set; }

		public virtual IEnumerable<Payment>? Payments { get; set; }
	}
}
