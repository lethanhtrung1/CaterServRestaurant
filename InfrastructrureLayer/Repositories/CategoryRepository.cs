using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository {
		public CategoryRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
