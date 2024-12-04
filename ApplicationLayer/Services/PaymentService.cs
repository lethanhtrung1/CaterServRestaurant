using ApplicationLayer.Common.Constants;
using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Payment;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Payment;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using ApplicationLayer.Momo.Requests;
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
		private readonly MomoOptions _momoOptions;
		private readonly ILogException _logger;

		public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService,
			IOptions<VnpayOptions> vnpayOptions, IOptions<MomoOptions> momoOptions, ILogException logger) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_currentUserService = currentUserService;
			_vnpayOptions = vnpayOptions.Value;
			_momoOptions = momoOptions.Value;
			_logger = logger;
		}

		public async Task<ApiResponse<PaymentLinkDto>> CreatePayment(CreatePaymentRequest request) {
			try {
				await _unitOfWork.Payment.BeginTransactionAsync();
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
					PaymentDestinationId = request.PaymentDestinationId,
					PaymentStatus = PaymentStatus.Unpaid,
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
				await _unitOfWork.Payment.EndTransactionAsync();

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
						var momoPaymentRequest = new MomoPaymentRequest(
							_momoOptions.PartnerCode,
							newPayment.Id.ToString() ?? string.Empty,
							(long)request.RequiredAmount!,
							newPayment.Id.ToString() ?? string.Empty,
							request.PaymentContent ?? string.Empty,
							_momoOptions.ReturnUrl,
							_momoOptions.IpnUrl,
							"captureWallet",
							string.Empty
						);

						momoPaymentRequest.MakeSignature(_momoOptions.AccessKey, _momoOptions.SecretKey);

						(bool createMomoLinkResult, string? createMessage) = momoPaymentRequest.GetLink(_momoOptions.PaymentUrl);

						if (createMomoLinkResult) {
							paymentUrl = createMessage;
						}

						break;

					default: break;
				}

				var result = new PaymentLinkDto {
					PaymentId = newPayment.Id,
					PaymentUrl = paymentUrl!
				};

				return new ApiResponse<PaymentLinkDto>(result, true, "Create payment successfully");
			} catch (Exception ex) {
				await _unitOfWork.Payment.RollBackTransactionAsync();
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

		public async Task<ApiResponse<(PaymentReturnDto, string)>> PaymentReturn(VnpayPayResponse request) {
			string returnUrl = string.Empty;
			var result = new ApiResponse<(PaymentReturnDto, string)>();

			await _unitOfWork.Payment.BeginTransactionAsync();
			try {
				//var orderId = Guid.Parse(request.vnp_TxnRef);
				//var order = await _unitOfWork.Order.GetAsync(x => x.Id == orderId);

				//if (order == null || order.OrderStatus == OrderStatus.Completed) {
				//	return new ApiResponse<(PaymentReturnDto, string)>(false, "Link invalid");
				//}

				var resultData = new PaymentReturnDto();
				var isValidSignature = request.IsValidSignature(_vnpayOptions.HashSecret);

				if (isValidSignature) {
					var paymentId = Guid.Parse(request.vnp_TxnRef);
					var payment = await _unitOfWork.Payment.GetAsync(x => x.Id == paymentId);

					if (payment != null) {
						var merchant = await _unitOfWork.Merchant.GetAsync(x => x.Id == payment.MerchantId);
						returnUrl = merchant.MerchantReturnUrl ?? "https://localhost:5173/payment";

						var order = await _unitOfWork.Order.GetAsync(x => x.Id == payment.OrderId);
						if (order == null || order.OrderStatus == OrderStatus.Completed) {
							return new ApiResponse<(PaymentReturnDto, string)>(false, "Link payment invalid");
						}

						// payment success
						if (request.vnp_ResponseCode == "00" && request.vnp_TransactionStatus == "00") {
							resultData.PaymentStatus = "00";
							resultData.PaymentId = paymentId;
							resultData.PaymentMessage = "Confirm success";
							resultData.Amount = request.vnp_Amount / 100;
							resultData.PaymentDate = DateTime.ParseExact(request.vnp_PayDate, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

							payment.PaymentStatus = PaymentStatus.Completed;

							// Update order status
							order.OrderStatus = OrderStatus.Completed;
							await _unitOfWork.Order.UpdateAsync(order);

							if (order.Booking != null) {
								var booking = await _unitOfWork.Booking.GetAsync(x => x.Id == order.BookingId);
								booking.Status = BookingStatus.Completed;
								await _unitOfWork.Booking.UpdateAsync(booking);

								var bookingTables = await _unitOfWork.BookingTable.GetListAsync(x => x.BookingId == booking.Id);
								foreach ( var bookingTable in bookingTables) {
									var table = await _unitOfWork.Table.GetAsync(x => x.Id == bookingTable.TableId);
									table.Status = TableStatus.Free;
									await _unitOfWork.Table.UpdateAsync(table);
								}
							}

							// return url
							returnUrl = $"{returnUrl}/confirm";
						}
						// Payment failed
						else {
							resultData.PaymentStatus = "10";
							resultData.PaymentMessage = "Payment process failed";

							payment!.PaymentStatus = PaymentStatus.Failed;

							// return url
							returnUrl = $"{returnUrl}/reject";
						}
					} else {
						resultData.PaymentStatus = "11";
						resultData.PaymentMessage = "Can't find payment";

						payment!.PaymentStatus = PaymentStatus.Failed;

						// return url
						returnUrl = $"{returnUrl}/reject";
					}

					await _unitOfWork.Payment.UpdateAsync(payment);
					await _unitOfWork.SaveChangeAsync();
					await _unitOfWork.Payment.EndTransactionAsync();

					result.Success = true;
					result.Message = "Confirm payment successfully";
					result.Data = (resultData, returnUrl);
				}
			} catch (Exception ex) {
				await _unitOfWork.Payment.RollBackTransactionAsync();
				result.Success = false;
				result.Message = ex.Message;
				result.Data = (null, string.Empty)!;
			}
			return result;
		}

		public async Task<ApiResponse<(PaymentReturnDto, string)>> MomoPaymentReturn(MomoPaymentResultRequest request) {
			string returnUrl = string.Empty;
			var result = new ApiResponse<(PaymentReturnDto, string)>();

			await _unitOfWork.Payment.BeginTransactionAsync();

			try {
				var resultData = new PaymentReturnDto();
				var isValidSignature = request.IsValidSignature(_momoOptions.AccessKey, _momoOptions.SecretKey);

				if (isValidSignature) {
					var paymentId = Guid.Parse(request.requestId);
					var payment = await _unitOfWork.Payment.GetAsync(x => x.Id == paymentId);

					if (payment != null) {
						var merchant = await _unitOfWork.Merchant.GetAsync(x => x.Id == payment.MerchantId);
						returnUrl = merchant.MerchantReturnUrl ?? "https://localhost:5173/payment";

						var order = await _unitOfWork.Order.GetAsync(x => x.Id == payment.OrderId);
						if (order == null || order.OrderStatus == OrderStatus.Completed) {
							return new ApiResponse<(PaymentReturnDto, string)>(false, "Link payment invalid");
						}

						// payment success
						if (request.resultCode == 0) {
							resultData.PaymentStatus = "00";
							resultData.PaymentId = paymentId;
							resultData.PaymentMessage = "Confirm success";
							resultData.Amount = request.amount;
							resultData.PaymentDate = DateTimeOffset.FromUnixTimeSeconds(request.responseTime).UtcDateTime;

							payment.PaymentStatus = PaymentStatus.Completed;

							// Update order status
							order.OrderStatus = OrderStatus.Completed;
							await _unitOfWork.Order.UpdateAsync(order);

							if (order.Booking != null) {
								var booking = await _unitOfWork.Booking.GetAsync(x => x.Id == order.BookingId);
								booking.Status = BookingStatus.Completed;
								await _unitOfWork.Booking.UpdateAsync(booking);

								var bookingTables = await _unitOfWork.BookingTable.GetListAsync(x => x.BookingId == booking.Id);
								foreach (var bookingTable in bookingTables) {
									var table = await _unitOfWork.Table.GetAsync(x => x.Id == bookingTable.TableId);
									table.Status = TableStatus.Free;
									await _unitOfWork.Table.UpdateAsync(table);
								}
							}

							// return url
							returnUrl = $"{returnUrl}/confirm";
						}
						// Payment failed
						else {
							resultData.PaymentStatus = "10";
							resultData.PaymentMessage = "Payment process failed";

							payment!.PaymentStatus = PaymentStatus.Failed;

							// return url
							returnUrl = $"{returnUrl}/reject";
						}
					} else {
						resultData.PaymentStatus = "11";
						resultData.PaymentMessage = "Can't find payment";

						payment!.PaymentStatus = PaymentStatus.Failed;

						// return url
						returnUrl = $"{returnUrl}/reject";
					}

					await _unitOfWork.Payment.UpdateAsync(payment);
					await _unitOfWork.SaveChangeAsync();
					await _unitOfWork.Payment.EndTransactionAsync();

					result.Success = true;
					result.Message = "Confirm payment successfully";
					result.Data = (resultData, returnUrl);
				}
			} catch (Exception ex) {
				await _unitOfWork.Payment.RollBackTransactionAsync();
				result.Success = false;
				result.Message = ex.Message;
				result.Data = (null, string.Empty)!;
			}

			return result;
		}

		public async Task<ApiResponse<PagedList<PaymentResponse>>> GetPaymentPaging(GetPaymentPagingRequest request) {
			try {
				var payments = await _unitOfWork.Payment.GetListAsync(includeProperties: "PaymentDestination");

				if (request.From != null) {
					payments = payments.Where(x => x.PaymentDate >= request.From).ToList();
				}

				if (request.To != null) {
					payments = payments.Where(x => x.PaymentDate <= request.From).ToList();
				}

				if (payments == null || !payments.Any()) {
					return new ApiResponse<PagedList<PaymentResponse>>(false, "No record available");
				}

				var paymentPaging = payments.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
				int total = payments.Count();

				var result = new List<PaymentResponse>();
				foreach (var payment in paymentPaging) {
					var paymentResponse = _mapper.Map<PaymentResponse>(payment);
					paymentResponse.PaymentDes = payment.PaymentDestination.DesShortName;
					result.Add(paymentResponse);
				}

				return new ApiResponse<PagedList<PaymentResponse>>(
					new PagedList<PaymentResponse>(result, request.PageNumber, request.PageSize, total),
					true, "Retrieve payments successfully"
				);
			} catch (Exception) {

				throw;
			}
		}

		public async Task<ApiResponse<PaymentResponse>> GetPaymentById(Guid id) {
			try {
				var payment = await _unitOfWork.Payment.GetAsync(x => x.Id == id, includeProperties: "PaymentDestination");

				if (payment == null) {
					return new ApiResponse<PaymentResponse>(false, "Payment not found");
				}

				var response = _mapper.Map<PaymentResponse>(payment);
				response.PaymentDes = payment.PaymentDestination.DesShortName;

				return new ApiResponse<PaymentResponse>(response, true, "Retieve payment successfully");
			} catch (Exception ex) {
				return new ApiResponse<PaymentResponse>(false, $"{ex.Message}");
			}
		}
	}
}
