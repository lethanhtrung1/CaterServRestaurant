using DomainLayer.Common;
using DomainLayer.Entites;

namespace DomainLayer.Repositories {
	public interface IReviewRepository : IRepositoryBase<Review> {
		Task<double?> GetAverageRating(Guid productId);
	}
}
