using ApplicationLayer.DTOs.Requests.Meal;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class MealController : ControllerBase {
		private readonly IMealService _service;

		public MealController(IMealService service) {
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> GetMeal(Guid id) {
			var result = await _service.GetMeal(id);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteMeal(Guid id) {
			return Ok(await _service.DeleteMeal(id));
		}

		[HttpDelete("remove-item")]
		public async Task<IActionResult> RemoveMealItem(RemoveMealProductRequest request) {
			var result = await _service.RemoveMealProduct(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> AddMealProduct(CreateMealProductRequest request) {
			var result = await _service.AddMealProduct(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("increase")]
		public async Task<IActionResult> IncreaseMealProduct(UpdateMealProductRequest request) {
			var result = await _service.IncreaseMealProduct(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut("reduce")]
		public async Task<IActionResult> ReduceMealProduct(UpdateMealProductRequest request) {
			var result = await _service.ReduceMealProduct(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}
	}
}
