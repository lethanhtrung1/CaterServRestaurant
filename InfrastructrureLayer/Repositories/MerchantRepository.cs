using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class MerchantRepository : RepositoryBase<Merchant>, IMerchantRepository {
		public MerchantRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
