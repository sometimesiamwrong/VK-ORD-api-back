using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.BrokenRules;

namespace Domain.Extensions
{
    public static class IHttpContextAccessorExtentions
    {
        public static Guid GetVkOrdCredentialId(this IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            var idStr = httpContext.Request.Headers["x-vkord-credential-id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(idStr))
            {
                throw BrokenRuleCodes.VkOrdCredentialsHeaderNotFound.AsExn();
            }

            if (!Guid.TryParse(idStr, out var credentialId))
            {
                throw new FormatException($"Invalid GUID format for x-vkord-credential-id: {idStr}");
            }

            return credentialId;
        }
    }
}