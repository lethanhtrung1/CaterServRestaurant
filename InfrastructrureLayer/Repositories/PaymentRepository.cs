using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class PaymentRepository : RepositoryBase<Payment>, IPaymentRepository {
		public PaymentRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
