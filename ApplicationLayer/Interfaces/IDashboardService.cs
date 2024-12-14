using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Dashboard;

namespace ApplicationLayer.Interfaces {
	public interface IDashboardService {
		Task<ApiResponse<DashboardDto>> GetDashboard(string criteria);
		Task<ApiResponse<List<LineChartDataDto>>> GetRevenueByMonth(int month);
		Task<ApiResponse<List<LineChartDataDto>>> GetRevenueAllMonth();
	}
}
