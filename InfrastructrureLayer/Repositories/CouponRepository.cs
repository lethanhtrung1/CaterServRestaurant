using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class CouponRepository : RepositoryBase<Coupon>, ICouponRepository {
		public CouponRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
