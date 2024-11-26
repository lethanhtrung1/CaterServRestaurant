using ApplicationLayer.DTOs.Requests.Review;
using ApplicationLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers {
	[Route("api/review")]
	[ApiController]
	public class ReviewController : ControllerBase {
		private readonly IReviewService _service;

		public ReviewController(IReviewService service) {
			_service = service;
		}

		[HttpGet("{id:Guid}")]
		public async Task<IActionResult> GetById(Guid id) {
			var result = await _service.GetReviewById(id);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("paging")]
		public async Task<IActionResult> GetPaging([FromQuery] GetPagingReviewsRequest request) {
			var result = await _service.GetPagingReviews(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpGet("average/{productId:Guid}")]
		public async Task<IActionResult> GetAverageRating(Guid productId) {
			var result = await _service.GetAverageRating(productId);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateRaing([FromBody] CreateReviewRequest request) {
			var result = await _service.CreateReview(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpPut]
		public async Task<IActionResult> UpdateRating([FromBody] UpdateReviewRequest request) {
			var result = await _service.UpdateReview(request);
			if (result == null || !result.Success) {
				return BadRequest(result);
			}
			return Ok(result);
		}

		[HttpDelete("{id:Guid}")]
		public async Task<IActionResult> Delete(Guid id) {
			var result = await _service.DeleteReview(id);
			if (!result) return BadRequest(result);
			return Ok(result);
		}
	}
}
