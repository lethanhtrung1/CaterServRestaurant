﻿using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Payment;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Payment;
using ApplicationLayer.Momo.Requests;
using ApplicationLayer.Vnpay.Responses;

namespace ApplicationLayer.Interfaces {
	public interface IPaymentService {
		ApiResponse<string> CreatePaymentUrl(CreatePaymentRequest request);
		Task<ApiResponse<PaymentLinkDto>> CreatePayment(CreatePaymentRequest request);
		Task<ApiResponse<(PaymentReturnDto, string)>> PaymentReturn(VnpayPayResponse request);
		Task<ApiResponse<(PaymentReturnDto, string)>> MomoPaymentReturn(MomoPaymentResultRequest request);
		Task<ApiResponse<PagedList<PaymentResponse>>> GetPaymentPaging(GetPaymentPagingRequest request);
		Task<ApiResponse<PaymentResponse>> GetPaymentById(Guid id);
	}
}
