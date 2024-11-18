using DomainLayer.Entites;
using DomainLayer.Repositories;
using InfrastructrureLayer.Common;
using InfrastructrureLayer.Data;

namespace InfrastructrureLayer.Repositories {
	public class PaymentDestinationRepository : RepositoryBase<PaymentDestination>, IPaymentDestinationRepository {
		public PaymentDestinationRepository(AppDbContext dbContext) : base(dbContext) {
		}
	}
}
