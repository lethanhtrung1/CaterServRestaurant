using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class InvoiceRepository : RepositoryBase<Invoice>, IInvoiceRepository {
		public InvoiceRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
