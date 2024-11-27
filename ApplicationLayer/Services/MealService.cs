﻿using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Requests.Meal;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Meal;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using DomainLayer.Exceptions;

namespace ApplicationLayer.Services {
	public class MealService : IMealService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;

		public MealService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_currentUserService = currentUserService;
		}

		public async Task<ApiResponse<MealResponse>> AddProductToMeal(CreateMealRequest request) {
			try {
				var customerId = _currentUserService.UserId;
				if (string.IsNullOrEmpty(customerId)) {
					throw new CustomDomainException("Invalid Customer");
				}
				await _unitOfWork.Meal.BeginTransactionAsync();
				//var checkMeal = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == customerId, includeProperties: "Meal");
				var checkMeal = await _unitOfWork.Meal.GetAsync(x => x.CustomerId == customerId);
				var response = new MealResponse();
				decimal totalAmount = 0;

				// Create new meal
				if (checkMeal == null) {
					var newMeal = new Meal {
						Id = Guid.NewGuid(), CustomerId = customerId,
						TotalPrice = 0, CreatedDate = DateTime.UtcNow,
					};

					await _unitOfWork.Meal.AddAsync(newMeal);

					// new mealProduct
					var mealProduct = new MealProduct {
						Id = Guid.NewGuid(),
						MealId = newMeal.Id,
						ProductId = request.ProductId,
						Quantity = request.Quantity,
					};
					var product = await _unitOfWork.Product.GetAsync(x => x.Id == request.ProductId);
					mealProduct.Price = mealProduct.Quantity * product.Price;
					totalAmount += mealProduct.Price;

					await _unitOfWork.MealProduct.AddAsync(mealProduct);

					newMeal.TotalPrice = totalAmount;
					await _unitOfWork.Meal.UpdateAsync(newMeal);

					// response
					response = _mapper.Map<MealResponse>(newMeal);
					var mealProductDetailResponse = _mapper.Map<MealProductDetailDto>(product);

					var mealProductReponse = new MealProductResponse {
						Id = mealProduct.Id,
						ProductDetail = mealProductDetailResponse,
						Quantity = mealProduct.Quantity,
						Price = mealProduct.Price,
					};
					var mealProductListResponse = new List<MealProductResponse>();
					mealProductListResponse.Add(mealProductReponse);
					response.Products = _mapper.Map<List<MealProductResponse>>(mealProductListResponse);
				}
				// meal already exists -> only add new meal product
				else {
					var checkMealProduct = await _unitOfWork.MealProduct.GetAsync(x => x.MealId == checkMeal.Id && x.ProductId == request.ProductId);

					// Add new MealProduct
					if (checkMealProduct == null) {
						var mealProduct = new MealProduct {
							Id = Guid.NewGuid(),
							MealId = checkMeal.Id,
							ProductId = request.ProductId,
							Quantity = request.Quantity,
						};
						var product = await _unitOfWork.Product.GetAsync(x => x.Id == request.ProductId);
						mealProduct.Price = mealProduct.Quantity * product.Price;
						totalAmount += mealProduct.Price;
						await _unitOfWork.MealProduct.AddAsync(mealProduct);
					}
					// Meal product already exists in Meal -> Update quantity MealProduct
					else {
						checkMealProduct.Quantity += request.Quantity;
						checkMealProduct.Price = checkMealProduct.Quantity * checkMealProduct.Price;
						totalAmount += checkMealProduct.Price;
						await _unitOfWork.MealProduct.UpdateAsync(checkMealProduct);
					}

					// Update Meal.TotalPrice
					var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == checkMeal.Id);
					meal.TotalPrice = totalAmount;
					await _unitOfWork.Meal.UpdateAsync(meal);

					// return response
					response = _mapper.Map<MealResponse>(meal);
					response.Products = new List<MealProductResponse>();

					// Get List meal product
					var listMealProduct = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == checkMeal.Id);

					// Mapp response MealProduct
					foreach (var item in listMealProduct) {
						var productDetail = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
						item.Price = productDetail.Price * item.Quantity;
						var productDto = _mapper.Map<MealProductDetailDto>(productDetail);

						var mealProductDto = _mapper.Map<MealProductResponse>(item);
						mealProductDto.ProductDetail = productDto;

						response.Products.Add(mealProductDto);
					}
					response.TotalPrice = response.Products.Select(x => x.Price).Sum();
				}

				await _unitOfWork.SaveChangeAsync();
				await _unitOfWork.Meal.EndTransactionAsync();

				return new ApiResponse<MealResponse>(response, true, "Created successfully");
			} catch (Exception ex) {
				await _unitOfWork.Meal.RollBackTransactionAsync();
				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		public async Task<bool> DeleteMeal(Guid id) {
			var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == id);
			if (meal == null) {
				return false;
			}
			var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == id);
			foreach (var mealProduct in mealProducts) {
				await _unitOfWork.MealProduct.RemoveAsync(mealProduct);
			}
			await _unitOfWork.Meal.RemoveAsync(meal);
			await _unitOfWork.SaveChangeAsync();
			return true;
		}

		public async Task<ApiResponse<MealResponse>> GetMeal(Guid id) {
			try {
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == id);
				if (meal == null) {
					return new ApiResponse<MealResponse>(false, "Meal not found");
				}

				var response = _mapper.Map<MealResponse>(meal);
				//response.TableId = meal.TableId;
				//response.TableName = meal.Table.Name;
				response.Products = new List<MealProductResponse>();

				var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == meal.Id);

				foreach (var item in mealProducts) {
					var productDetail = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
					item.Price = productDetail.Price * item.Quantity;
					var productDto = _mapper.Map<MealProductDetailDto>(productDetail);

					var mealProductDto = _mapper.Map<MealProductResponse>(item);
					mealProductDto.ProductDetail = productDto;

					response.Products.Add(mealProductDto);
				}

				response.TotalPrice = response.Products.Select(x => x.Price).Sum();

				return new ApiResponse<MealResponse>(response, true, "");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		public async Task<ApiResponse<MealResponse>> GetMealByCustomerId() {
			try {
				var customerId = _currentUserService.UserId;
				var meal = await _unitOfWork.Meal.GetAsync(x => x.CustomerId == customerId);
				if (meal == null) {
					return new ApiResponse<MealResponse>(false, "Meal not found");
				}

				var response = _mapper.Map<MealResponse>(meal);
				//response.TableId = meal.TableId;
				//response.TableName = meal.Table.Name;
				response.Products = new List<MealProductResponse>();

				var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == meal.Id);

				foreach (var item in mealProducts) {
					var productDetail = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
					item.Price = productDetail.Price * item.Quantity;
					var productDto = _mapper.Map<MealProductDetailDto>(productDetail);

					var mealProductDto = _mapper.Map<MealProductResponse>(item);
					mealProductDto.ProductDetail = productDto;

					response.Products.Add(mealProductDto);
				}

				response.TotalPrice = response.Products.Select(x => x.Price).Sum();

				return new ApiResponse<MealResponse>(response, true, "");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		//public async Task<ApiResponse<MealResponse>> AddMealProduct(CreateMealProductRequest request) {
		//	try {
		//		var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId, includeProperties: "Table,MealProducts");
		//		if (meal == null) {
		//			return new ApiResponse<MealResponse>(false, "Meal not found");
		//		}
		//		decimal totalAmount = 0;

		//		if (meal.MealProducts.Count() == 0) {
		//			foreach (var item in request.Products) {
		//				var mealProduct = new MealProduct {
		//					Id = Guid.NewGuid(),
		//					MealId = request.MealId,
		//					ProductId = item.ProductId,
		//					Quantity = item.Quantity,
		//				};
		//				var product = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
		//				mealProduct.Price = mealProduct.Quantity * product.Price;
		//				totalAmount += mealProduct.Price;
		//				await _unitOfWork.MealProduct.AddAsync(mealProduct);
		//			}
		//		} else {
		//			foreach (var item in request.Products) {
		//				var checkMealProduct = await _unitOfWork.MealProduct.GetAsync(x => x.MealId == request.MealId);
		//				// Check product has been added to the meal -> update quantity
		//				var mealProductFromDb = await _unitOfWork.MealProduct.GetAsync(x => x.MealId == request.MealId && x.ProductId == item.ProductId);
		//				if (mealProductFromDb != null) {
		//					mealProductFromDb.Quantity += item.Quantity;
		//					mealProductFromDb.Price = mealProductFromDb.Quantity * mealProductFromDb.Price;
		//					totalAmount += mealProductFromDb.Price;
		//					await _unitOfWork.MealProduct.UpdateAsync(mealProductFromDb);
		//				}
		//				// add new MealProduct
		//				else {
		//					var mealProduct = new MealProduct {
		//						Id = Guid.NewGuid(),
		//						MealId = request.MealId,
		//						ProductId = item.ProductId,
		//						Quantity = item.Quantity,
		//					};
		//					var product = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
		//					mealProduct.Price = mealProduct.Quantity * product.Price;
		//					totalAmount += mealProduct.Price;
		//					await _unitOfWork.MealProduct.AddAsync(mealProduct);
		//				}
		//			}
		//		}
		//		meal.TotalPrice = totalAmount;
		//		await _unitOfWork.Meal.UpdateAsync(meal);
		//		await _unitOfWork.SaveChangeAsync();

		//		// Response
		//		var response = _mapper.Map<MealResponse>(meal);
		//		//response.TableId = meal.TableId;
		//		//response.TableName = meal.Table.Name;
		//		response.Products = new List<MealProductResponse>();

		//		var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == meal.Id);

		//		foreach (var item in mealProducts) {
		//			var productDetail = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
		//			item.Price = productDetail.Price * item.Quantity;
		//			var productDto = _mapper.Map<MealProductDetailDto>(productDetail);

		//			// if thumbnail is not null -> return full path
		//			if (!string.IsNullOrEmpty(productDto.Thumbnail)) {
		//				productDto.Thumbnail = Path.Combine(Directory.GetCurrentDirectory(), productDto.Thumbnail);
		//			}

		//			var mealProductDto = _mapper.Map<MealProductResponse>(item);
		//			mealProductDto.ProductDetail = productDto;

		//			response.Products.Add(mealProductDto);
		//		}

		//		response.TotalPrice = response.Products.Select(x => x.Price).Sum();

		//		return new ApiResponse<MealResponse>(response, true, "Add product successfully");
		//	} catch (Exception ex) {

		//		return new ApiResponse<MealResponse>(false, $"{ex.Message}");
		//	}
		//}

		public async Task<ApiResponse<MealResponse>> RemoveMealProduct(RemoveMealProductRequest request) {
			try {
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId);
				if (meal == null) {
					return new ApiResponse<MealResponse>(false, "Meal not found");
				}

				var mealProduct = await _unitOfWork.MealProduct.GetAsync(x => x.MealId == request.MealId && x.Id == request.MealProductId);
				if (mealProduct == null) {
					return new ApiResponse<MealResponse>(false, "MealProduct not found");
				}

				await _unitOfWork.MealProduct.RemoveAsync(mealProduct);
				await _unitOfWork.SaveChangeAsync();

				// Response
				var response = _mapper.Map<MealResponse>(meal);
				//response.TableId = meal.TableId;
				//response.TableName = meal.Table.Name;
				response.Products = new List<MealProductResponse>();

				var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == meal.Id);

				foreach (var item in mealProducts) {
					var productDetail = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
					item.Price = productDetail.Price * item.Quantity;
					var productDto = _mapper.Map<MealProductDetailDto>(productDetail);

					// if thumbnail is not null -> return full path
					if (!string.IsNullOrEmpty(productDto.Thumbnail)) {
						productDto.Thumbnail = Path.Combine(Directory.GetCurrentDirectory(), productDto.Thumbnail);
					}

					var mealProductDto = _mapper.Map<MealProductResponse>(item);
					mealProductDto.ProductDetail = productDto;

					response.Products.Add(mealProductDto);
				}

				response.TotalPrice = response.Products.Select(x => x.Price).Sum();

				return new ApiResponse<MealResponse>(response, true, "Remove product successfully");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		public async Task<ApiResponse<MealResponse>> IncreaseMealProduct(UpdateMealProductRequest request) {
			try {
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId);
				if (meal == null) {
					return new ApiResponse<MealResponse>(false, "Meal not found");
				}

				var mealProduct = await _unitOfWork.MealProduct
					.GetAsync(x => x.MealId == request.MealId && x.Id == request.MealProductId, includeProperties: "Product");
				if (mealProduct == null) {
					return new ApiResponse<MealResponse>(false, "MealProduct not found");
				}

				mealProduct.Quantity += 1;
				mealProduct.Price = mealProduct.Quantity * mealProduct.Product.Price;
				await _unitOfWork.MealProduct.UpdateAsync(mealProduct);
				await _unitOfWork.SaveChangeAsync();

				// Response
				var response = _mapper.Map<MealResponse>(meal);
				//response.TableId = meal.TableId;
				//response.TableName = meal.Table.Name;
				response.Products = new List<MealProductResponse>();

				var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == meal.Id);

				foreach (var item in mealProducts) {
					var productDetail = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
					item.Price = productDetail.Price * item.Quantity;
					var productDto = _mapper.Map<MealProductDetailDto>(productDetail);

					var mealProductDto = _mapper.Map<MealProductResponse>(item);
					mealProductDto.ProductDetail = productDto;

					response.Products.Add(mealProductDto);
				}

				response.TotalPrice = response.Products.Select(x => x.Price).Sum();

				return new ApiResponse<MealResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		public async Task<ApiResponse<MealResponse>> ReduceMealProduct(UpdateMealProductRequest request) {
			try {
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId);
				if (meal == null) {
					return new ApiResponse<MealResponse>(false, "Meal not found");
				}

				var mealProduct = await _unitOfWork.MealProduct
					.GetAsync(x => x.MealId == request.MealId && x.Id == request.MealProductId, includeProperties: "Product");
				if (mealProduct == null) {
					return new ApiResponse<MealResponse>(false, "MealProduct not found");
				}

				if (mealProduct.Quantity == 1) {
					await _unitOfWork.MealProduct.RemoveAsync(mealProduct);
				} else {
					mealProduct.Quantity -= 1;
					mealProduct.Price = mealProduct.Quantity * mealProduct.Product.Price;
					await _unitOfWork.MealProduct.UpdateAsync(mealProduct);
				}
				await _unitOfWork.SaveChangeAsync();

				// Response
				var response = _mapper.Map<MealResponse>(meal);
				//response.TableId = meal.TableId;
				//response.TableName = meal.Table.Name;
				response.Products = new List<MealProductResponse>();

				var mealProducts = await _unitOfWork.MealProduct.GetListAsync(x => x.MealId == meal.Id);

				foreach (var item in mealProducts) {
					var productDetail = await _unitOfWork.Product.GetAsync(x => x.Id == item.ProductId);
					item.Price = productDetail.Price * item.Quantity;
					var productDto = _mapper.Map<MealProductDetailDto>(productDetail);

					var mealProductDto = _mapper.Map<MealProductResponse>(item);
					mealProductDto.ProductDetail = productDto;

					response.Products.Add(mealProductDto);
				}

				response.TotalPrice = response.Products.Select(x => x.Price).Sum();

				return new ApiResponse<MealResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}
	}
}
