using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class MealProductRepository : RepositoryBase<MealProduct>, IMealProductRepository {
		public MealProductRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
