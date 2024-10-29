using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class MenuRepository : RepositoryBase<Menu>, IMenuRepository {
		public MenuRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
