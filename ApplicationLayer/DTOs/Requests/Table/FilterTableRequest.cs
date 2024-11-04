using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Table {
	public class FilterTableRequest : PagingRequest {
		public int MaxCapacity { get; set; }
		public bool IsAvailable { get; set; }
		public string? AreaName { get; set; }
	}
}
