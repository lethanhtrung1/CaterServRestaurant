using DomainLayer.Entites;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace DomainLayer.Common {
	public interface IRepositoryBase<T> where T : BaseEntity {
		Task<IDbContextTransaction> BeginTransactionAsync();
		Task EndTransactionAsync();
		Task RollBackTransactionAsync();

		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task RemoveAsync(T entity);
		Task RemoveRangeAsync(IEnumerable<T> entities);
		Task<T> GetAsync(Expression<Func<T, bool>> predicate);
		Task<T> GetAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null, bool tracked = false);
		Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null);
	}
}
