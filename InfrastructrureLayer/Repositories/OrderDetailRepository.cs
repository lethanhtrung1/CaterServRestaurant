using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository {
		public OrderDetailRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
