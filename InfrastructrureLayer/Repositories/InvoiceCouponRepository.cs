using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class InvoiceCouponRepository : RepositoryBase<InvoiceCoupon>, IInvoiceCouponRepository {
		public InvoiceCouponRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
