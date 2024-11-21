using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Requests.Payment;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Payment;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using ApplicationLayer.Options;
using ApplicationLayer.Vnpay.Requests;
using ApplicationLayer.Vnpay.Responses;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using Microsoft.Extensions.Options;

namespace ApplicationLayer.Services {
	public class PaymentService : IPaymentService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;
		private readonly VnpayOptions _vnpayOptions;
		private readonly ILogException _logger;

		public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService,
			IOptions<VnpayOptions> vnpayOptions, ILogException logger) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_currentUserService = currentUserService;
			_vnpayOptions = vnpayOptions.Value;
			_logger = logger;
		}

		public async Task<ApiResponse<PaymentLinkDto>> CreatePayment(CreatePaymentRequest request) {
			try {
				var newPayment = new Payment {
					Id = Guid.NewGuid(),
					PaymentContent = request.PaymentContent,
					PaymentCurrency = request.PaymentCurrency,
					RequiredAmount = request.RequiredAmount,
					PaymentDate = DateTime.Now,
					ExpireDate = DateTime.Now.AddMinutes(15),
					PaymentLanguage = request.PaymentLanguage,
					OrderId = request.OrderId,
					MerchantId = request.MerchantId,
					PaymentDestinationId = request.PaymentDestinationId
				};
				await _unitOfWork.Payment.AddAsync(newPayment);

				var newPaymentSign = new PaymentSignature {
					Id = Guid.NewGuid(),
					//SignValue = request.Signature,
					SignValue = Guid.NewGuid().ToString().Substring(0, 8),
					SignDate = DateTime.Now,
					SignOwn = request.MerchantId.ToString(),
					PaymentId = newPayment.Id,
					IsValid = true,
				};
				await _unitOfWork.PaymentSignature.AddAsync(newPaymentSign);
				await _unitOfWork.SaveChangeAsync();

				var paymentUrl = string.Empty;

				switch (request.PaymentDesname) {
					case "VNPAY":
						var vnpayPayRequest = new VnpayPayRequest(_vnpayOptions.Version,
							_vnpayOptions.TmnCode,
							DateTime.Now,
							_currentUserService.IpAddress ?? string.Empty,
							request.RequiredAmount ?? 0,
							request.PaymentCurrency ?? string.Empty,
							"other",
							request.PaymentContent ?? string.Empty,
							_vnpayOptions.ReturnUrl,
							newPayment.Id.ToString() ?? string.Empty // paymentId
						);
						paymentUrl = vnpayPayRequest.GetLink(_vnpayOptions.PaymentUrl, _vnpayOptions.HashSecret);

						break;

					case "MOMO":
						// handle logic momo
						break;

					default: break;
				}

				var result = new PaymentLinkDto {
					PaymentId = newPayment.Id,
					PaymentUrl = paymentUrl
				};

				return new ApiResponse<PaymentLinkDto>(result, true, "Create payment successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				throw new Exception("An error occurred while creating payment", ex);
			}
		}

		public ApiResponse<string> CreatePaymentUrl(CreatePaymentRequest request) {
			try {
				string paymentUrl = string.Empty;
				switch (request.PaymentDesname) {
					case "VNPAY":
						var vnpayPayRequest = new VnpayPayRequest(_vnpayOptions.Version,
							_vnpayOptions.TmnCode,
							DateTime.Now,
							_currentUserService.IpAddress ?? string.Empty,
							request.RequiredAmount ?? 0,
							request.PaymentCurrency ?? string.Empty,
							"other",
							request.PaymentContent ?? string.Empty,
							_vnpayOptions.ReturnUrl,
							request.OrderId.ToString() ?? string.Empty // paymentId
						);
						paymentUrl = vnpayPayRequest.GetLink(_vnpayOptions.PaymentUrl, _vnpayOptions.HashSecret);

						break;

					case "MOMO":
						// handle logic momo
						break;

					default: break;
				}

				return new ApiResponse<string>(paymentUrl, true, "Create payment url successfully");
			} catch (Exception ex) {
				throw new Exception("An error occurred while creating payemt url", ex);
			}
		}

		public Task<ApiResponse<(PaymentReturnDto, string)>> PaymentReturn(VnpayPayResponse request) {
			throw new NotImplementedException();
		}
	}
}
