using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class PaymentSignatureRepository : RepositoryBase<PaymentSignature>, IPaymentSignatureRepository {
		public PaymentSignatureRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
