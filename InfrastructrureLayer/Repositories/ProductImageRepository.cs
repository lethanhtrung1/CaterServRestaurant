using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class ProductImageRepository : RepositoryBase<ProductImage>, IProductImageRepository {
		public ProductImageRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
