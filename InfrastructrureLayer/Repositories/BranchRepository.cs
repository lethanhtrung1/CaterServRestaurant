using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class BranchRepository : RepositoryBase<Branch>, IBranchRepository {
		public BranchRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
