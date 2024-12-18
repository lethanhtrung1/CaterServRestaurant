﻿using ApplicationLayer.DTOs.Pagination;

namespace ApplicationLayer.DTOs.Requests.Table {
	public class FilterTableRequest : PagingRequest {
		public string? Status { get; set; }
		public int MaxCapacity { get; set; } = 0;
		public bool IsAvailable { get; set; } = true;
		public string? AreaName { get; set; }
	}
}
