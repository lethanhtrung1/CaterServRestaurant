using ApplicationLayer.DTOs.Requests.Meal;
using ApplicationLayer.DTOs.Responses;
using ApplicationLayer.DTOs.Responses.Meal;

namespace ApplicationLayer.Interfaces {
	public interface IMealService {
		Task<ApiResponse<MealResponse>> AddProductToMeal(CreateMealRequest request);
		Task<bool> DeleteMeal(Guid id);
		Task<ApiResponse<MealResponse>> GetMeal(Guid id);
		Task<ApiResponse<MealResponse>> IncreaseMealProduct(UpdateMealProductRequest request);
		Task<ApiResponse<MealResponse>> ReduceMealProduct(UpdateMealProductRequest request);
		//Task<ApiResponse<MealResponse>> AddMealProduct(CreateMealProductRequest request);
		Task<ApiResponse<MealResponse>> RemoveMealProduct(RemoveMealProductRequest request);
	}
}
