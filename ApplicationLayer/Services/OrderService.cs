using ApplicationLayer.Common.Constants;
using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Pagination;
using ApplicationLayer.DTOs.Requests.Order;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Order;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Logging;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;

namespace ApplicationLayer.Services {
	public class OrderService : IOrderService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICurrentUserService _currentUserService;
		private readonly IMapper _mapper;
		private readonly ILogException _logger;

		public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService,
			ILogException logger) {
			_currentUserService = currentUserService;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<ApiResponse<OrderResponse>> CreateOrder(CreateOrderRequest request) {
			try {
				var customerId = _currentUserService.UserId;
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId);
				if (meal == null) return new ApiResponse<OrderResponse>(false, "Meal not found");

				var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == request.MealId, includeProperties: "Product");
				if (mealProducts == null || !mealProducts.Any()) {
					return new ApiResponse<OrderResponse>(false, "Meal is empty");
				}

				await _unitOfWork.Order.BeginTransactionAsync();

				// Dine in
				if (request.OrderType == 1) {
					var booking = await _unitOfWork.Booking.GetLatestBookingByTableIdAsync(request.TableId);
					if (booking == null) return new ApiResponse<OrderResponse>(false, "Booking not found");
					var order = await _unitOfWork.Order.GetAsync(x => x.BookingId == booking.Id);

					// create new order
					if (order == null) {
						var newOrder = new Order() {
							Id = Guid.NewGuid(),
							OrderType = request.OrderType,
							OrderStatus = OrderStatus.Pending,
							CreatedDate = DateTime.Now,
							LastUpdatedAt = DateTime.Now,
							ShippingDate = DateTime.Now.AddMinutes(40),
							CustomerId = customerId,
							CustomerName = request.CustomerName,
							CustomerPhone = request.CustomerPhone,
							ShippingAddress = request.ShippingAddress,
							DiscountAmount = request.DiscountAmount,
							TotalAmount = meal.TotalPrice,
						};
						await _unitOfWork.Order.AddAsync(newOrder);

						var response = _mapper.Map<OrderResponse>(newOrder);
						response.OrderTypeName = OrderType.DineIn;
						response.OrderDetails = new List<OrderDetailResponse>();

						// Create new OrderDetails
						foreach (var item in mealProducts) {
							var orderDetai = new OrderDetail {
								Id = Guid.NewGuid(),
								ProductId = item.ProductId,
								ProductName = item.Product.Name,
								OrderId = newOrder.Id,
								UnitName = item.Product.UnitName!,
								Price = item.Product.SellingPrice,
								Quantity = item.Quantity,
								TotalPrice = item.Price,
								CreatedAt = DateTime.Now,
							};
							await _unitOfWork.OrderDetail.AddAsync(orderDetai);

							response.OrderDetails.Add(_mapper.Map<OrderDetailResponse>(orderDetai));
						}
						// Remove Meal & MealProduct
						await _unitOfWork.MealProduct.RemoveRangeAsync(mealProducts);
						await _unitOfWork.Meal.RemoveAsync(meal);
						await _unitOfWork.SaveChangeAsync();
						await _unitOfWork.Order.EndTransactionAsync();

						response.OrderDetails = response.OrderDetails.OrderByDescending(x => x.CreatedAt).ToList();
						return new ApiResponse<OrderResponse>(response, true, "Create order successfully");
					}
					// Update order
					else {
						var totalOrderPrice = order.TotalAmount;
						var response = _mapper.Map<OrderResponse>(order);
						response.OrderDetails = new List<OrderDetailResponse>();

						foreach (var item in mealProducts) {
							var orderDetai = new OrderDetail {
								Id = Guid.NewGuid(),
								ProductId = item.ProductId,
								ProductName = item.Product.Name,
								OrderId = order.Id,
								UnitName = item.Product.UnitName!,
								Price = item.Product.SellingPrice,
								Quantity = item.Quantity,
								TotalPrice = item.Price,
								CreatedAt = DateTime.Now,
							};
							totalOrderPrice += orderDetai.TotalPrice;
							await _unitOfWork.OrderDetail.AddAsync(orderDetai);

							response.OrderDetails.Add(_mapper.Map<OrderDetailResponse>(orderDetai));
						}
						order.TotalAmount = totalOrderPrice;
						order.LastUpdatedAt = DateTime.Now;
						await _unitOfWork.Order.UpdateAsync(order);

						// Remove Meal & MealProduct
						await _unitOfWork.MealProduct.RemoveRangeAsync(mealProducts);
						await _unitOfWork.Meal.RemoveAsync(meal);
						await _unitOfWork.SaveChangeAsync();
						await _unitOfWork.Order.EndTransactionAsync();

						response.OrderDetails = response.OrderDetails.OrderByDescending(x => x.CreatedAt).ToList();
						return new ApiResponse<OrderResponse>(response, true, "Create Order successfully");
					}
				}
				// Takeaway / Delivery
				else {
					var newOrder = new Order() {
						Id = Guid.NewGuid(),
						OrderType = request.OrderType,
						OrderStatus = OrderStatus.Pending,
						CreatedDate = DateTime.Now,
						LastUpdatedAt = DateTime.Now,
						ShippingDate = DateTime.Now.AddMinutes(40),
						CustomerId = customerId,
						CustomerName = request.CustomerName,
						CustomerPhone = request.CustomerPhone,
						ShippingAddress = request.ShippingAddress,
						DiscountAmount = request.DiscountAmount,
						TotalAmount = meal.TotalPrice,
					};
					await _unitOfWork.Order.AddAsync(newOrder);
					var response = _mapper.Map<OrderResponse>(newOrder);
					response.OrderTypeName = OrderType.TakeAway;
					response.OrderDetails = new List<OrderDetailResponse>();

					foreach (var item in mealProducts) {
						var orderDetai = new OrderDetail {
							Id = Guid.NewGuid(),
							ProductId = item.ProductId,
							ProductName = item.Product.Name,
							OrderId = newOrder.Id,
							UnitName = item.Product.UnitName!,
							Price = item.Product.SellingPrice,
							Quantity = item.Quantity,
							TotalPrice = item.Price,
							CreatedAt = DateTime.Now,
						};
						await _unitOfWork.OrderDetail.AddAsync(orderDetai);

						response.OrderDetails.Add(_mapper.Map<OrderDetailResponse>(orderDetai));
					}
					await _unitOfWork.MealProduct.RemoveRangeAsync(mealProducts);
					await _unitOfWork.Meal.RemoveAsync(meal);
					await _unitOfWork.SaveChangeAsync();
					await _unitOfWork.Order.EndTransactionAsync();

					response.OrderDetails = response.OrderDetails.OrderByDescending(x => x.CreatedAt).ToList();
					return new ApiResponse<OrderResponse>(response, true, "Create order successfully");
				}
			} catch (Exception ex) {
				await _unitOfWork.Order.RollBackTransactionAsync();
				_logger.LogExceptions(ex);
				return new ApiResponse<OrderResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<bool> CancelOrder(Guid orderId) {
			try {
				var order = await _unitOfWork.Order.GetAsync(x => x.Id == orderId);

				if (order == null) {
					return false;
				}

				order.OrderStatus = OrderStatus.Cancel;
				await _unitOfWork.Order.UpdateAsync(order);
				await _unitOfWork.SaveChangeAsync();

				return true;
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				throw new Exception("An error occurred while canceling the order", ex);
			}
		}

		public async Task<ApiResponse<OrderResponse>> GetOrderById(Guid orderId) {
			try {
				var order = await _unitOfWork.Order.GetAsync(x => x.Id == orderId);

				if (order == null) {
					return new ApiResponse<OrderResponse>(false, $"Order with Id: {orderId} not found");
				}

				var orderDetails = await _unitOfWork.OrderDetail.GetListAsync(x => x.OrderId == orderId);

				var response = _mapper.Map<OrderResponse>(order);
				response.OrderDetails = _mapper.Map<List<OrderDetailResponse>>(orderDetails);

				if (response.OrderType == 1) response.OrderTypeName = "Phục vụ tại bàn";
				else if (response.OrderType == 2) response.OrderTypeName = "Mang về";
				else if (response.OrderType == 3) response.OrderTypeName = "Giao hàng";
				else response.OrderTypeName = "";

				response.OrderDetails = response.OrderDetails.OrderByDescending(x => x.CreatedAt).ToList();
				return new ApiResponse<OrderResponse>(response, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<OrderResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<OrderResponse>> GetOrderByBooking(Guid bookingId) {
			try {
				var order = await _unitOfWork.Order.GetAsync(x => x.BookingId == bookingId);

				if (order == null) {
					return new ApiResponse<OrderResponse>(false, $"Order with BookingId: {bookingId} not found");
				}

				var orderDetails = await _unitOfWork.OrderDetail.GetListAsync(x => x.OrderId == order.Id);

				var response = _mapper.Map<OrderResponse>(order);
				response.OrderDetails = _mapper.Map<List<OrderDetailResponse>>(orderDetails);

				if (response.OrderType == 1) response.OrderTypeName = "Phục vụ tại bàn";
				else if (response.OrderType == 2) response.OrderTypeName = "Mang về";
				else if (response.OrderType == 3) response.OrderTypeName = "Giao hàng";
				else response.OrderTypeName = "";

				response.OrderDetails = response.OrderDetails.OrderByDescending(x => x.CreatedAt).ToList();
				return new ApiResponse<OrderResponse>(response, true, "");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<OrderResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<OrderResponse>>> GetOrders(GetOrdersPagingRequest request) {
			try {
				var orders = await _unitOfWork.Order.GetListAsync();

				orders = orders.OrderByDescending(x => x.LastUpdatedAt).ToList();

				if (!string.IsNullOrEmpty(request.OrderStatus)) {
					orders = orders.Where(x => x.OrderStatus == request.OrderStatus).ToList();
				}

				if (request.SortBy != 0) {
					if (request.SortBy == OrderByField.ASC) orders = orders.OrderBy(x => x.TotalAmount).ToList();
					else if (request.SortBy == OrderByField.DESC) orders = orders.OrderByDescending(x => x.TotalAmount).ToList();
				}

				var ordersPagedList = orders.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
				if (ordersPagedList == null || !ordersPagedList.Any()) {
					return new ApiResponse<PagedList<OrderResponse>>(false, "No record available");
				}

				int totalRecord = orders.Count();
				var result = _mapper.Map<List<OrderResponse>>(ordersPagedList);

				foreach (var order in result) {
					if (order.OrderType == 1) order.OrderTypeName = "Phục vụ tại bàn";
					else if (order.OrderType == 2) order.OrderTypeName = "Mang về";
					else if (order.OrderType == 3) order.OrderTypeName = "Giao hàng";
					else order.OrderTypeName = "";
				}

				return new ApiResponse<PagedList<OrderResponse>>(
					new PagedList<OrderResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, "Retrieve orders successfully"
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<OrderResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<PagedList<OrderResponse>>> GetOrdersByUserId(string userId, GetOrdersPagingRequest request) {
			try {
				var orders = await _unitOfWork.Order.GetListAsync(x => x.CustomerId == userId);

				if (!string.IsNullOrEmpty(request.OrderStatus)) {
					orders = orders.Where(x => x.OrderStatus == request.OrderStatus).ToList();
				}

				if (request.SortBy != 0) {
					if (request.SortBy == OrderByField.ASC) orders = orders.OrderBy(x => x.TotalAmount).ToList();
					else if (request.SortBy == OrderByField.DESC) orders = orders.OrderByDescending(x => x.TotalAmount).ToList();
				}

				var ordersPagedList = orders.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
				if (ordersPagedList == null || !ordersPagedList.Any()) {
					return new ApiResponse<PagedList<OrderResponse>>(false, "No record available");
				}

				int totalRecord = orders.Count();
				var result = _mapper.Map<List<OrderResponse>>(ordersPagedList);
				result = result.OrderByDescending(x => x.LastUpdatedAt).ToList();

				foreach (var order in result) {
					if (order.OrderType == 1) order.OrderTypeName = "Phục vụ tại bàn";
					else if (order.OrderType == 2) order.OrderTypeName = "Mang về";
					else if (order.OrderType == 3) order.OrderTypeName = "Giao hàng";
					else order.OrderTypeName = "";
				}

				return new ApiResponse<PagedList<OrderResponse>>(
					new PagedList<OrderResponse>(result, request.PageNumber, request.PageSize, totalRecord),
					true, "Retrieve your orders successfully"
				);
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<PagedList<OrderResponse>>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<OrderResponse>> UpdateOrderStatus(UpdateOrderStatusRequest request) {
			try {
				var order = await _unitOfWork.Order.GetAsync(x => x.Id == request.Id);

				if (order == null) {
					return new ApiResponse<OrderResponse>(false, "Order not found");
				}

				order.OrderStatus = request.Status;
				await _unitOfWork.Order.UpdateAsync(order);
				await _unitOfWork.SaveChangeAsync();

				var result = _mapper.Map<OrderResponse>(order);
				if (result.OrderType == 1) result.OrderTypeName = "Phục vụ tại bàn";
				else if (result.OrderType == 2) result.OrderTypeName = "Mang về";
				else if (result.OrderType == 3) result.OrderTypeName = "Giao hàng";
				else result.OrderTypeName = "";

				return new ApiResponse<OrderResponse>(result, true, "Update status successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<OrderResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<OrderResponse>> CreateOrderByBooking(Guid bookingId) {
			try {
				var booking = await _unitOfWork.Booking.GetAsync(x => x.Id == bookingId);

				if (booking == null) {
					return new ApiResponse<OrderResponse>(false, "Booking not found");
				}

				var order = new Order {
					Id = Guid.NewGuid(),
					BookingId = bookingId,
					CustomerId = booking.CustomerId != null ? booking.CustomerId : null,
					CreatedDate = DateTime.Now,
					LastUpdatedAt = DateTime.Now,
					ShippingDate = null,
					ShippingAddress = string.Empty,
					CustomerName = booking.CustomerName,
					CustomerPhone = booking.Phone,
					DeliveryAmount = 0,
					DepositAmount = 0,
					DiscountAmount = 0,
					TotalAmount = 0,
					OrderType = 1,
					OrderStatus = OrderStatus.Processing,
				};

				await _unitOfWork.Order.AddAsync(order);
				await _unitOfWork.SaveChangeAsync();

				var response = _mapper.Map<OrderResponse>(order);

				return new ApiResponse<OrderResponse>(response, true, "Create order successfully");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<OrderResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}

		public async Task<ApiResponse<OrderResponse>> AddOrderOrderDetail(CreateOrderDetailRequest request) {
			try {
				var checkOrder = await _unitOfWork.Order.GetAsync(x => x.Id == request.OrderId);
				if (checkOrder == null) {
					return new ApiResponse<OrderResponse>(false, "Order not found");
				}

				var product = await _unitOfWork.Product.GetAsync(x => x.Id == request.ProductId);
				if (product == null) {
					return new ApiResponse<OrderResponse>(false, "Product not found");
				}

				var newOrderDetail = new OrderDetail {
					Id = Guid.NewGuid(),
					OrderId = request.OrderId,
					ProductId = product.Id,
					ProductName = product.Name,
					Quantity = request.Quantity,
					Price = product.SellingPrice,
					TotalPrice = request.Quantity * product.SellingPrice,
					UnitName = product.UnitName!,
					CreatedAt = DateTime.Now,
				};

				await _unitOfWork.OrderDetail.AddAsync(newOrderDetail);
				checkOrder.TotalAmount += newOrderDetail.TotalPrice;
				checkOrder.LastUpdatedAt = DateTime.Now;
				await _unitOfWork.Order.UpdateAsync(checkOrder);
				await _unitOfWork.SaveChangeAsync();

				var orderDetails = await _unitOfWork.OrderDetail.GetListAsync(x => x.OrderId == checkOrder.Id);

				var orderDetailsResponse = _mapper.Map<List<OrderDetailResponse>>(orderDetails);
				var orderResponse = _mapper.Map<OrderResponse>(checkOrder);

				orderResponse.OrderDetails = orderDetailsResponse;

				return new ApiResponse<OrderResponse>(orderResponse, true, "Success");
			} catch (Exception ex) {
				_logger.LogExceptions(ex);
				return new ApiResponse<OrderResponse>(false, $"Internal server error occurred: {ex.Message}");
			}
		}
	}
}
