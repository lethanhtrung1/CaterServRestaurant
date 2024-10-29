using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class InvoiceDetailRepository : RepositoryBase<InvoiceDetail>, IInvoiceDetailRepository {
		public InvoiceDetailRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
