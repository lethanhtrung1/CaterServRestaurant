namespace DomainLayer.Entites {
	public class Invoice : BaseEntity {
		public Guid BranchId { get; set; }
		public Guid OrderId { get; set; }

		/// <summary>
		/// 1 Đơn hàng phục vụ tại nhà hàng, 2 Đơn hàng gói mang về, 3 Đơn hàng giao tận nơi, 4	Đặt chỗ trước
		/// </summary>
		public int OrderType { get; set; }
		public string CustomerId { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public string CustomerPhone { get; set; } = string.Empty;

		public string EmployeeId { get; set; }
		public string EmployeeName { get; set; } = string.Empty;
		public string? ShippingAddress { get; set; }
		public string TableName { get; set; } = string.Empty;
		public string? Description { get; set; }
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Tiền đặt cọc
		/// </summary>
		public decimal DepositAmount { get; set; }

		/// <summary>
		/// Tiền hàng
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// Phí giao hàng
		/// </summary>
		public decimal DeliveryAmount { get; set; }

		/// <summary>
		/// Giảm giá theo hóa đơn
		/// </summary>
		public decimal PromotionAmount { get; set; }

		/// <summary>
		/// Giảm giá các mặt hàng
		/// </summary>
		public decimal PromotionItemsAmount { get; set; }

		/// <summary>
		/// Số tiền đã trả
		/// </summary>
		public decimal ReceiveAmount { get; set; }

		/// <summary>
		/// Số tiền trả lại cho khách
		/// </summary>
		public decimal ReturnAmount { get; set; }

		/// <summary>
		/// Tổng thanh toán
		/// </summary>
		public decimal TotalAmount { get; set; }

		/// <summary>
		/// Doanh số
		/// </summary>
		public decimal SaleAmount { get; set; }

		/// <summary>
		/// 1 Chưa thanh toán, 2 Chưa thu tiền, 3 Đã thu tiền, 4 Đã hủy, 5 Tạm hủy
		/// </summary>
		public int PaymentStatus { get; set; }

		/// <summary>
		/// Số điểm có trước khi thanh toán hóa đơn
		/// </summary>
		public int AvailablePoint {  get; set; }

		/// <summary>
		/// Số điểm sử dụng trong hóa đơn
		/// </summary>
		public int UsedPoint { get; set; }

		/// <summary>
		/// Số điểm tích được trong hóa đơn
		/// </summary>
		public int AddPoint { get; set; }

		public Branch? Branch { get; set; }
		public Order? Order { get; set; }
		public virtual IEnumerable<InvoiceDetail> InvoiceDetails { get; set; }
		public InvoiceCoupon? InvoiceCoupon { get; set; }
		public InvoicePayment? InvoicePayment { get; set; }
	}
}
