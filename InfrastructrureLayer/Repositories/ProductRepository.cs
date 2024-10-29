using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class ProductRepository : RepositoryBase<Product>, IProductRepository {
		public ProductRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
