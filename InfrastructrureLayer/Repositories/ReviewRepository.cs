using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace InfrastructrureLayer.Repositories {
	public class ReviewRepository : RepositoryBase<Review>, IReviewRepository {
		private readonly AppDbContext _dbContext;

		public ReviewRepository(AppDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public async Task<double?> GetAverageRating(Guid productId) {
			var reviews = await _dbContext.Reviews.Where(x => x.ProductId == productId).ToListAsync();

			if (!reviews.Any()) return null;

			return reviews.Average(x => x.Rating);
		}
	}
}
