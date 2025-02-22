﻿using ApplicationLayer.Common.Constants;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Dashboard;
using ApplicationLayer.Interfaces;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class DashboardService : IDashboardService {
		private readonly IUnitOfWork _unitOfWork;

		public DashboardService(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResponse<DashboardDto>> GetDashboard(string criteria) {
			var result = new DashboardDto();

			try {
				result.TotalUsers = await _unitOfWork.ApplicationUser.GetTotalUsers();

				DateTime startDate = criteria.ToLower() switch {
					"week" => DateTime.UtcNow.AddDays(-7),
					"month" => DateTime.UtcNow.AddDays(-30),
					_ => throw new ArgumentException("Invalid criteria", nameof(criteria))
				};

				DateTime endDate = DateTime.UtcNow;

				// Get total orders
				var orders = await _unitOfWork.Order
					.GetListAsync(x => x.CreatedDate.Date >= startDate.Date && x.CreatedDate.Date <= endDate.Date);
				result.TotalOrder = orders.Count();

				// Get total payments
				var payments = await _unitOfWork.Payment.GetListAsync(
					x => x.PaymentStatus == PaymentStatus.Completed &&
						 x.PaymentDate.Date >= startDate.Date && x.PaymentDate.Date <= endDate.Date);
				result.TotalRevenueAdmount = payments.Sum(x => x.RequiredAmount) ?? 0;

				// Line Chart
				result.TotalRevenue = GenerateLineChartData(payments, startDate, endDate);

				// Get order details
				var orderDetails = await _unitOfWork.OrderDetail.GetListAsync(
					x => x.CreatedAt.Date >= startDate.Date && x.CreatedAt.Date <= endDate.Date,
					includeProperties: "Order,Product,Product.Menu,Product.Category");

				var totalSum = orderDetails.Where(x => x.Order.OrderStatus == OrderStatus.Completed).Sum(x => x.TotalPrice);

				// Pie chart group by Menu
				result.ProductsGroupedByMenu = GeneratePieChartData(orderDetails, x => x.Product.Menu.MenuName, totalSum);
				// Pie chart group by Category
				result.ProductsGroupedByCategory = GeneratePieChartData(orderDetails, x => x.Product.Category.Name, totalSum);
			} catch (Exception ex) {
				throw new Exception($"{ex.Message}");
			}

			return new ApiResponse<DashboardDto>(result, true, "Retrieve successfully");
		}

		private List<LineChartDataDto> GenerateLineChartData(IEnumerable<Payment> payments, DateTime startDate, DateTime endDate) {
			var allDates = Enumerable.Range(0, (endDate.Date - startDate.Date).Days + 1)
				.Select(offset => startDate.Date.AddDays(offset))
				.ToList();

			// group data by date
			var groupedPayments = payments
				.GroupBy(x => x.PaymentDate.Date)
				.ToDictionary(g => g.Key, g => g.Sum(y => y.RequiredAmount ?? 0));

			var lineChartData = allDates.Select(date => new LineChartDataDto {
				Label = date.ToString("dd/MM/yyyy"),
				Value = groupedPayments.ContainsKey(date) ? groupedPayments[date] : 0
			}).OrderBy(x => DateTime.ParseExact(x.Label, "dd/MM/yyyy", null))
			.ToList();

			return lineChartData;
		}

		private List<PieChartDataDto> GeneratePieChartData(
			IEnumerable<OrderDetail> orderDetails,
			Func<OrderDetail, string> groupBySelector,
			decimal totalSum) {

			return orderDetails
				.Where(x => x.Order.OrderStatus == OrderStatus.Completed)
				.GroupBy(groupBySelector)
				.Select(group => new PieChartDataDto {
					Label = group.Key,
					Value = totalSum == 0 ? 0 : Math.Round((group.Sum(y => y.TotalPrice) / totalSum) * 100, 2)
				})
				.ToList();
		}

		public async Task<ApiResponse<List<LineChartDataDto>>> GetRevenueAllMonth() {
			try {
				var result = new List<LineChartDataDto>();

				var payments = await _unitOfWork.Payment.GetListAsync();

				result = payments
					.Where(x => x.PaymentStatus == PaymentStatus.Completed)
					.GroupBy(g => g.PaymentDate.Month)
					.Select(s => new LineChartDataDto {
						Label = s.Key.ToString(),
						Value = s.Sum(y => y.RequiredAmount ?? 0)
					})
					.ToList();

				return new ApiResponse<List<LineChartDataDto>>(result, true, "Retrieve successfully");
			} catch (Exception ex) {
				throw new Exception($"{ex.Message}");
			}
		}

		//public async Task<ApiResponse<List<LineChartDataDto>>> GetRevenueAllMonth() {
		//	try {
		//		var result = new List<LineChartDataDto>();

		//		// Get all payments
		//		var payments = await _unitOfWork.Payment.GetListAsync();

		//		// Prepare data grouped by month and year
		//		var groupedPayments = payments
		//			.Where(x => x.PaymentStatus == PaymentStatus.Completed && x.PaymentDate.Year == DateTime.UtcNow.Year)
		//			.GroupBy(g => new { g.PaymentDate.Year, g.PaymentDate.Month })
		//			.Select(s => new {
		//				Year = s.Key.Year,
		//				Month = s.Key.Month,
		//				TotalRevenue = s.Sum(y => y.RequiredAmount ?? 0)
		//			})
		//			.ToDictionary(x => (x.Year, x.Month), x => x.TotalRevenue);

		//		// Ensure all months are present for each year in the range
		//		for (int month = 1; month <= DateTime.UtcNow.Year; month++) {
		//			var label = $"{month:D2}/{DateTime.UtcNow.Year}";
		//			result.Add(new LineChartDataDto {
		//				Label = label,
		//				Value = groupedPayments.ContainsKey((DateTime.UtcNow.Year, month)) ? groupedPayments[(DateTime.UtcNow.Year, month)] : 0
		//			});
		//		}

		//		return new ApiResponse<List<LineChartDataDto>>(result, true, "Retrieve successfully");
		//	} catch (Exception ex) {
		//		throw new Exception($"{ex.Message}");
		//	}
		//}

		public async Task<ApiResponse<List<LineChartDataDto>>> GetRevenueByMonth(int month) {
			try {
				var result = new List<LineChartDataDto>();

				var payments = await _unitOfWork.Payment.GetListAsync();

				result = payments
					.Where(x => x.PaymentStatus == PaymentStatus.Completed && x.PaymentDate.Month == month)
					.GroupBy(g => g.PaymentDate.Date)
					.Select(s => new LineChartDataDto {
						Label = s.Key.ToString("dd/MM/yyyy"),
						Value = s.Sum(y => y.RequiredAmount ?? 0)
					})
					.ToList();

				return new ApiResponse<List<LineChartDataDto>>(result, true, "Retrieve successfully");
			} catch (Exception ex) {
				throw new Exception($"{ex.Message}");
			}
		}
	}
}
