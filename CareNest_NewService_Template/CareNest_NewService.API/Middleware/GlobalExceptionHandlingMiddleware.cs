using CareNest_NewService.Domain.Commons.Base;
using System.Net;
using System.Text.Json;

namespace CareNest_NewService.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by middleware.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            object errorDetails;

            if (exception is BaseException.ErrorException errorException)
            {
                statusCode = errorException.StatusCode;
                errorDetails = new
                {
                    title = errorException.ErrorDetail.ErrorCode,
                    details = errorException.ErrorDetail.ErrorMessage
                };
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorDetails = new
                {
                    title = "An unexpected error occurred.",
                    details = exception.Message
                };
            }

            var response = new
            {
                error = errorDetails,
                statusCode,
                timestamp = DateTime.UtcNow
            };

            string payload = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(payload);
        }
    }
}
