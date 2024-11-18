using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructrureLayer.Repositories {
	public class BookingRepository : RepositoryBase<Booking>, IBookingRepository {
		private readonly AppDbContext _dbContext;

		public BookingRepository(AppDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public async Task<Booking?> GetLatestBookingByTableIdAsync(Guid? tableId) {
			if (tableId == null || tableId == Guid.Empty) {
				return null;
			}

			var latestBooking = await _dbContext.BookingsTables
				.Where(bt => bt.TableId == tableId)
				.Include(bt => bt.Booking) // Load Booking để tránh NullReferenceException
				.Select(bt => bt.Booking)
				.OrderByDescending(b => b.CheckinTime)
				.FirstOrDefaultAsync();

			return latestBooking;
		}
	}
}
