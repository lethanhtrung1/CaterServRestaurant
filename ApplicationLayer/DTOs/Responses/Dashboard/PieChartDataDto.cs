namespace ApplicationLayer.DTOs.Responses.Dashboard {
	public class PieChartDataDto {
		public string Label { get; set; } = string.Empty;
		public decimal Value { get; set; }

		public PieChartDataDto() { }

		public PieChartDataDto(string label, decimal value) {
			this.Label = label;

			this.Value = Math.Round(value, 2);
		}
	}
}
