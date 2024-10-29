using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class BookingRepository : RepositoryBase<Booking>, IBookingRepository {
		public BookingRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
