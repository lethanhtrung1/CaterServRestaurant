using System.Linq.Expressions;

namespace DomainLayer.Common {
	public interface IRepository<T> where T : class {
		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task RemoveAsync(T entity);
		Task RemoveRangeAsync(IEnumerable<T> entities);
		Task<T> GetAsync(Expression<Func<T, bool>> predicate);
		Task<T> GetAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null, bool tracked = false);
		Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null);
		Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
	}
}
