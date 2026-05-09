using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace redil_backend.Middlewares
{
    public class ApiKeyFilter : IAsyncActionFilter
    {
        private const string ApiKeyHeader = "api-key";
        private readonly IConfiguration _config;

        public ApiKeyFilter(IConfiguration config) => _config = config;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hasAttribute = context.ActionDescriptor.EndpointMetadata
                .Any(m => m is ApiKeyAttribute);

            if (!hasAttribute)
            {
                await next();
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var key)
                || key != _config["Jwt:Secret"])
            {
                context.Result = new UnauthorizedObjectResult("API Key inválida.");
                return;
            }

            await next();
        }
    }
}
