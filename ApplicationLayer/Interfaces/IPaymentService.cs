using ApplicationLayer.DTOs.Requests.Payment;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Payment;
using ApplicationLayer.Vnpay.Responses;

namespace ApplicationLayer.Interfaces {
	public interface IPaymentService {
		ApiResponse<string> CreatePaymentUrl(CreatePaymentRequest request);
		Task<ApiResponse<PaymentLinkDto>> CreatePayment(CreatePaymentRequest request);
		Task<ApiResponse<(PaymentReturnDto, string)>> PaymentReturn(VnpayPayResponse request);
	}
}
