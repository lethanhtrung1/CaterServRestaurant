using DomainLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Common {
	public class UnitOfWork : IUnitOfWork {
		private readonly AppDbContext _dbContext;

		public UnitOfWork(AppDbContext dbContext) {
			_dbContext = dbContext;
		}

		public async Task SaveChangeAsync() {
			await _dbContext.SaveChangesAsync();
		}
		
		public void Dispose() {
			_dbContext.Dispose();
		}
	}
}
