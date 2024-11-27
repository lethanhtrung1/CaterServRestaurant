namespace ApplicationLayer.DTOs.Responses.Account {
	public record GeneralResponse(bool IsSuccess = false, string Message = null!);
}
