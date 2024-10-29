using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class OrderRepository : RepositoryBase<Order>, IOrderRepository {
		public OrderRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
