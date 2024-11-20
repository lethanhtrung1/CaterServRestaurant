namespace ApplicationLayer.DTOs.Pagination {
	public class PagedList<T> : PagedResultBase {
		public IList<T> Items { get; set; } = new List<T>();
		public PagedList(IList<T> items, int pageNumber, int pageSize, int count) {
			TotalRecord = count;
			Items = items;
			PageSize = pageSize;
			PageNumber = pageNumber;
		}
	}
}
