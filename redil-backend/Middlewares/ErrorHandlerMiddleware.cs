using redil_backend.Dtos.Responses;
using System.Runtime.CompilerServices;

namespace redil_backend.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
                _logger.LogError(ex, "Error no controlado");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<int>
                {
                    Success = false,
                    Message = "Error inesperado de servidor"
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
