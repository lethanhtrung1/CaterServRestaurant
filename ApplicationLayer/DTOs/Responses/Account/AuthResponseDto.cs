﻿namespace ApplicationLayer.DTOs.Responses.Account {
	public class AuthResponseDto {
		public bool Success { get; set; }
		public string? Message { get; set; }

		public AuthResponseDto(bool success = false, string? message = null) {
			Success = success;
			Message = message;
		}
	}
}