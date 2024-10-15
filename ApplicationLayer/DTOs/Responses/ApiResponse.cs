namespace ApplicationLayer.DTOs.Responses {
	public class ApiResponse<T> {
		public T? Data { get; set; }
		public bool Success { get; set; } = true;
		public string Message { get; set; } = string.Empty;

		public ApiResponse(T data, bool success, string message) {
			Data = data;
			Success = success;
			Message = message;
		}

		public ApiResponse(bool success, string message) {
			Success = success;
			Message = message;
		}
	}
}
