namespace ApplicationLayer.DTOs.Responses.Dashboard {
	public class LineChartDataDto {
		public string Label { get; set; } = string.Empty;
		public decimal Value { get; set; }

		public LineChartDataDto() { }

		public LineChartDataDto(string label, int value) {
			Label = label;
			Value = value;
		}
	}
}
