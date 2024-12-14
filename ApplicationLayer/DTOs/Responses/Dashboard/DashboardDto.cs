namespace ApplicationLayer.DTOs.Responses.Dashboard {
	public class DashboardDto {
		public int TotalUsers { get; set; }
		public int TotalOrder { get; set; }
		public decimal TotalRevenueAdmount { get; set; }
		public List<LineChartDataDto>? TotalRevenue { get; set; }
		public List<PieChartDataDto>? ProductsGroupedByCategory { get; set; }
		public List<PieChartDataDto>? ProductsGroupedByMenu { get; set; }
	}
}
