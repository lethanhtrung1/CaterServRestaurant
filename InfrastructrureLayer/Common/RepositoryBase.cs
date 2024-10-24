using DomainLayer.Common;
using DomainLayer.Entites;
using InfrastructrureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace InfrastructrureLayer.Common {
	public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity {
		private readonly AppDbContext _dbContext;
		private readonly DbSet<T> _dbSet;

		public RepositoryBase(AppDbContext dbContext) {
			_dbContext = dbContext;
			_dbSet = _dbContext.Set<T>();
		}

		public async Task AddAsync(T entity) {
			await _dbContext.AddAsync(entity);
		}

		public Task<IDbContextTransaction> BeginTransactionAsync()
			=> _dbContext.Database.BeginTransactionAsync();

		public async Task EndTransactionAsync() {
			await _dbContext.SaveChangesAsync();
			await _dbContext.Database.CommitTransactionAsync();
		}

		public async Task<T> GetAsync(Expression<Func<T, bool>> predicate) {
			IQueryable<T> query = _dbSet.Where(predicate);
			return await query.FirstAsync();
		}

		public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null, bool tracked = false) {
			IQueryable<T> query = tracked ? _dbSet : _dbSet.AsNoTracking();
			query = query.Where(predicate);
			if (!string.IsNullOrEmpty(includeProperties)) {
				foreach (var property in includeProperties.Split(new Char[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
					query = query.Include(property);
				}
			}
			return await query.FirstAsync();
		}

		public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null) {
			IQueryable<T> query = _dbSet;
			if (predicate != null) {
				query = query.Where(predicate);
			}
			if (!string.IsNullOrEmpty(includeProperties)) {
				foreach (var property in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
					query = query.Include(property);
				}
			}
			return await query.ToListAsync();
		}

		public Task RemoveAsync(T entity) {
			_dbContext.Set<T>().Remove(entity);
			return Task.CompletedTask;
		}

		public Task RemoveRangeAsync(IEnumerable<T> entities) {
			_dbContext.Set<T>().RemoveRange(entities);
			return Task.CompletedTask;
		}

		public Task RollBackTransactionAsync() => _dbContext.Database.RollbackTransactionAsync();

		public Task UpdateAsync(T entity) {
			if (_dbContext.Entry(entity).State == EntityState.Unchanged) {
				return Task.CompletedTask;
			}

			T exist = _dbContext.Set<T>().Find(entity.Id)!;
			_dbContext.Entry(exist).CurrentValues.SetValues(entity);

			return Task.CompletedTask;
		}
	}
}
