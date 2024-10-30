namespace ApplicationLayer.DTOs.Pagination {
	public class PagingRequest {
		private int _pageNumber;
		private int _pageSize;

		public int PageNumber {
			get { return _pageNumber == 0 ? 1 : _pageNumber; }
			set { _pageNumber = value; }
		}

		public int PageSize {
			get { return _pageSize == 0 ? 10 : _pageSize; }
			set { _pageSize = value; }
		}
	}
}
