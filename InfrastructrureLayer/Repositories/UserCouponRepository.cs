using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class UserCouponRepository : RepositoryBase<UserCoupon>, IUserCouponRepository {
		public UserCouponRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
