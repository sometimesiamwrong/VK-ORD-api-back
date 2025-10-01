using System.Security.Claims;

namespace VkOrdApiWrapper.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (Guid.TryParse(id, out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid or missing user id claim");
        }
    }
}


