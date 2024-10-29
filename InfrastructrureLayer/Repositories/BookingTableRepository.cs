using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class BookingTableRepository : RepositoryBase<BookingTable>, IBookingTableRepository {
		public BookingTableRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
