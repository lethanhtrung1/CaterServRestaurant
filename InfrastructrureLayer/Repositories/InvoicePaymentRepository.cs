using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class InvoicePaymentRepository : RepositoryBase<InvoicePayment>, IInvoicePaymentRepository {
		public InvoicePaymentRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
