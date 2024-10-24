namespace DomainLayer.Entites {
	public class Branch : BaseEntity {
		public string Code { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Location {  get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		/// <summary>
		/// True - là kho tổng
		/// </summary>
		public bool IsBaseDepot { get; set; }
		/// <summary>
		/// True - là chuỗi nhà hàng
		/// </summary>
		public bool IsChainBranch { get; set; }

		public virtual IEnumerable<Menu> Menus { get; set; }
	}
}
