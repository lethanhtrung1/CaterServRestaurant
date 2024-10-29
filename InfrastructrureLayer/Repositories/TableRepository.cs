using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class TableRepository : RepositoryBase<Table>, ITableRepository {
		public TableRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
