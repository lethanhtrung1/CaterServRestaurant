using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class MealRepository : RepositoryBase<Meal>, IMealRepository {
		public MealRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
