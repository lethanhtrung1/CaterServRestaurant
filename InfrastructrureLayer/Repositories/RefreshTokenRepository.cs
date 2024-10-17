using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository {
		public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
