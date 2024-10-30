namespace ApplicationLayer.DTOs.Pagination {
	public partial class PagedList<T> : List<T>, IPagedList<T> {
		public PagedList(IList<T> source, int pageIndex, int pageSize, int? totalCount = null) {
			pageSize = Math.Max(pageSize, 1);

			TotalCount = totalCount ?? source.Count;
			TotalPages = TotalCount / pageSize;

			if (TotalCount % pageSize > 0) TotalPages++;

			PageSize = pageSize;
			PageIndex = pageIndex;
			AddRange(totalCount != null ? source : source.Skip(PageIndex + 1).Take(pageSize));
		}

		public int PageIndex { get; set; }

		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public int TotalPages { get; set; }

		public bool HasPreviousPage => PageIndex > 0;

		public bool HasNextPage => PageIndex + 1 < TotalPages;
	}
}
