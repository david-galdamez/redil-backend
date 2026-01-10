using System.Security.Claims;

namespace redil_backend.Services
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user) =>
            int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        public static int? GetRedilId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst("redil_id")?.Value;
            return value == null ? null : int.Parse(value);
        }
    }
}
