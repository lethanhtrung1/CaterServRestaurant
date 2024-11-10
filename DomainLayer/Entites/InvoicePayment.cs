namespace DomainLayer.Entites {
	public class InvoicePayment : BaseEntity {
		public decimal Amount { get; set; }
		public string CustomerId { get; set; }
		public string CustomerName { get; set; }
		/// <summary>
		/// 1 Tiền mặt, 2 Thẻ ATM
		/// </summary>
		public int PaymentType { get; set; }
		public string PaymentName { get; set; }
		public string CardId { get; set; }
		public string CardName { get; set; }
		public string BankName { get; set; }
		public string BankAccountNumber { get; set; }
		public Guid InvoiceId { get; set; }

		public Invoice Invoice { get; set; }
		public ApplicationUser Customer { get; set; }
	}
}
