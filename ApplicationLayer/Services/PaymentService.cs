using ApplicationLayer.Interfaces;
using DomainLayer.Common;

namespace ApplicationLayer.Services {
	public class PaymentService : IPaymentService {
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}
	}
}
