using System.Security.Claims;

namespace Domain.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (long.TryParse(id, out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid or missing user id claim");
        }
    }
}


