using ApplicationLayer.Common.Consumer;
using ApplicationLayer.DTOs.Requests.Meal;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Meal;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common;
using DomainLayer.Entites;
using DomainLayer.Exceptions;

namespace ApplicationLayer.Services
{
    public class MealService : IMealService {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ICurrentUserService _currentUserService;

		public MealService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_currentUserService = currentUserService;
		}

		public async Task<ApiResponse<MealResponse>> CreateMeal(CreateMealRequest request) {
			try {
				var customerId = _currentUserService.UserId;

				if (string.IsNullOrEmpty(customerId)) {
					throw new CustomDomainException("Invalid Customer");
				}

				var meal = new Meal {
					Id = Guid.NewGuid(),
					CustomerId = customerId,
					TableId = request.TableId,
					TotalPrice = 0,
					CreatedDate = DateTime.UtcNow,
				};

				await _unitOfWork.Meal.AddAsync(meal);
				await _unitOfWork.SaveChangeAsync();

				var table = await _unitOfWork.Table.GetAsync(x => x.Id == request.TableId);

				var response = _mapper.Map<MealResponse>(meal);
				response.TableId = table.Id;
				response.TableName = table.Name;

				return new ApiResponse<MealResponse>(response, true, "Created successfully");
			} catch (Exception ex) {

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
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == id, includeProperties: "Table");
				if (meal == null) {
					return new ApiResponse<MealResponse>(false, "Meal not found");
				}

				var response = _mapper.Map<MealResponse>(meal);
				//response.TableId = meal.TableId;
				response.TableName = meal.Table.Name;
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

				return new ApiResponse<MealResponse>(response, true, "");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		public async Task<ApiResponse<MealResponse>> AddMealProduct(CreateMealProductRequest request) {
			try {
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId, includeProperties: "Table");
				if (meal == null) {
					return new ApiResponse<MealResponse>(false, "Meal not found");
				}

				foreach (var productId in request.ProductIds) {
					// Check product has been added to the meal -> update quantity
					var mealProductFromDb = await _unitOfWork.MealProduct.GetAsync(x => x.MealId == request.MealId && x.ProductId == productId);
					if (mealProductFromDb != null) {
						mealProductFromDb.Quantity += request.Quantity;
						mealProductFromDb.Price = mealProductFromDb.Quantity * mealProductFromDb.Product.Price;
						await _unitOfWork.MealProduct.UpdateAsync(mealProductFromDb);
					}
					// add new MealProduct
					else {
						var mealProduct = new MealProduct {
							Id = Guid.NewGuid(),
							MealId = request.MealId,
							ProductId = productId,
							Quantity = request.Quantity,
						};
						var product = await _unitOfWork.Product.GetAsync(x => x.Id == productId);
						mealProduct.Price = mealProduct.Quantity * product.Price;

						await _unitOfWork.MealProduct.AddAsync(mealProduct);
					}
				}
				await _unitOfWork.SaveChangeAsync();

				// Response
				var response = _mapper.Map<MealResponse>(meal);
				//response.TableId = meal.TableId;
				response.TableName = meal.Table.Name;
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

				return new ApiResponse<MealResponse>(response, true, "Add product successfully");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		public async Task<ApiResponse<MealResponse>> RemoveMealProduct(RemoveMealProductRequest request) {
			try {
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId, includeProperties: "Table");
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
				response.TableName = meal.Table.Name;
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
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId, includeProperties: "Table");
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
				response.TableName = meal.Table.Name;
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

				return new ApiResponse<MealResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}

		public async Task<ApiResponse<MealResponse>> ReduceMealProduct(UpdateMealProductRequest request) {
			try {
				var meal = await _unitOfWork.Meal.GetAsync(x => x.Id == request.MealId, includeProperties: "Table");
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
				response.TableName = meal.Table.Name;
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

				return new ApiResponse<MealResponse>(response, true, "Updated successfully");
			} catch (Exception ex) {

				return new ApiResponse<MealResponse>(false, $"{ex.Message}");
			}
		}
	}
}
