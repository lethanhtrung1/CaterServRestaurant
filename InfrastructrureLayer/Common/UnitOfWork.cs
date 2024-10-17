using DomainLayer.Common;
using DomainLayer.Repositories;
using InfrastructrureLayer.Data;
using InfrastructrureLayer.Repositories;

namespace InfrastructrureLayer.Common {
	public class UnitOfWork : IUnitOfWork {
		private readonly AppDbContext _dbContext;
		public IRefreshTokenRepository RefreshToken {  get; set; }

		public UnitOfWork(AppDbContext dbContext) {
			_dbContext = dbContext;
			RefreshToken = new RefreshTokenRepository(_dbContext);
		}

		public async Task SaveChangeAsync() {
			await _dbContext.SaveChangesAsync();
		}
		
		public void Dispose() {
			_dbContext.Dispose();
		}
	}
}
