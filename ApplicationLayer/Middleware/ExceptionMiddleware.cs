using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ApplicationLayer.Middleware {
	public partial class ExceptionMiddleware {
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) {
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext httpContext) {
			string message = "Sorry, internal server error occurred. Kindly try again.";
			int statusCode = (int)HttpStatusCode.InternalServerError;

			try {
				await _next(httpContext);

				// Check if Exception is Too Many Request - 429 status code.
				if (httpContext.Response.StatusCode == StatusCodes.Status429TooManyRequests) {
					message = "Too many request made.";
					statusCode = (int)StatusCodes.Status429TooManyRequests;
					await HandleExceptionAsync(httpContext, message, statusCode);
				}

				// If response is Unauthorized - 401
				if (httpContext.Response.StatusCode == StatusCodes.Status401Unauthorized) {
					message = "You are not authorized to access";
					statusCode = (int)StatusCodes.Status401Unauthorized;
					await HandleExceptionAsync(httpContext, message, statusCode);
				}

				// If Response is Forbidden - 403
				if (httpContext.Response.StatusCode == StatusCodes.Status403Forbidden) {
					message = "You are not allowed/request to access";
					statusCode = (int)StatusCodes.Status403Forbidden;
					await HandleExceptionAsync(httpContext, message, statusCode);
				}
			} catch (Exception ex) {
				// Log exception
				_logger.LogError($"Some thing went wrong: {ex}");

				// Exception is timeout - 408 request timeout
				if (httpContext.Response.StatusCode == StatusCodes.Status408RequestTimeout) {
					message = "Request timeout. Try again";
					statusCode = (int)StatusCodes.Status408RequestTimeout;
				}

				// Exception is caught
				// If none of the Exceptions then do the default
				await HandleExceptionAsync(httpContext, message, statusCode);
			}
		}

		private static Task HandleExceptionAsync(HttpContext httpContext, string message, int statusCode) {
			httpContext.Response.ContentType = "application/json";

			// Customize Message
			return httpContext.Response.WriteAsync(new ErrorDetails() {
				StatusCode = statusCode,
				Message = $"Message: Internal Server Error from the custom Middleware.{Environment.NewLine}Exception: {message}]"
			}.ToString());
		}
	}
}
