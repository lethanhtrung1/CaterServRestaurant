using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InfrastructrureLayer.Repositories {
	public class ApplicationUserRepository : IApplicationUserRepository {
		private readonly AppDbContext _dbContext;
		private readonly DbSet<ApplicationUser> _dbSet;

		public ApplicationUserRepository(AppDbContext dbContext) {
			_dbContext = dbContext;
			_dbSet = _dbContext.Set<ApplicationUser>();
		}

		public async Task AddAsync(ApplicationUser entity) {
			await _dbSet.AddAsync(entity);
		}

		public async Task AddRangeAsync(IEnumerable<ApplicationUser> entities) {
			await _dbSet.AddRangeAsync(entities);
		}

		public async Task<bool> AnyAsync(Expression<Func<ApplicationUser, bool>> predicate) {
			IQueryable<ApplicationUser> query = _dbSet;
			return await query.AnyAsync(predicate);
		}

		public async Task<ApplicationUser> GetAsync(Expression<Func<ApplicationUser, bool>> predicate) {
			IQueryable<ApplicationUser> query = _dbSet.Where(predicate);
			return await query.FirstOrDefaultAsync();
		}

		public async Task<ApplicationUser> GetAsync(Expression<Func<ApplicationUser, bool>> predicate, string? includeProperties = null, bool tracked = false) {
			IQueryable<ApplicationUser> query = tracked ? _dbSet : _dbSet.AsNoTracking();
			query = query.Where(predicate);
			if (!string.IsNullOrEmpty(includeProperties)) {
				foreach (var property in includeProperties.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
					query = query.Include(property);
				}
			}
			return await query.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<ApplicationUser>> GetListAsync(Expression<Func<ApplicationUser, bool>>? predicate = null, string? includeProperties = null) {
			IQueryable<ApplicationUser> query = _dbSet;
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

		public Task RemoveAsync(ApplicationUser entity) {
			_dbContext.Set<ApplicationUser>().Remove(entity);
			return Task.CompletedTask;
		}

		public Task RemoveRangeAsync(IEnumerable<ApplicationUser> entities) {
			_dbContext.Set<ApplicationUser>().RemoveRange(entities);
			return Task.CompletedTask;
		}

		public Task UpdateAsync(ApplicationUser entity) {
			if (_dbContext.Entry(entity).State == EntityState.Unchanged) {
				return Task.CompletedTask;
			}

			ApplicationUser exist = _dbContext.Set<ApplicationUser>().Find(entity.Id)!;
			_dbContext.Entry(exist).CurrentValues.SetValues(entity);

			return Task.CompletedTask;
		}
	}
}
