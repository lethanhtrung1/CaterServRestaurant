using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/dashboard")]
	[ApiController]
	public class DashboardController : ControllerBase {
		private readonly IDashboardService _dashboardService;

		public DashboardController(IDashboardService dashboardService) {
			_dashboardService = dashboardService;
		}

		[HttpGet("{criteria}")]
		public async Task<IActionResult> Get(string criteria) {
			var result = await _dashboardService.GetDashboard(criteria);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("line-chart/revenue")]
		public async Task<IActionResult> GetRevenueAllMonth() {
			var result = await _dashboardService.GetRevenueAllMonth();
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("line-chart/revenue/{month:int}")]
		public async Task<IActionResult> GetRevenueByMonth(int month) {
			var result = await _dashboardService.GetRevenueByMonth(month);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
