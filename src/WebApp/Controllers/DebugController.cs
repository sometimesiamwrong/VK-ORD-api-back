using Domain;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Controllers.Filters;
using WebApp.Security;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
public class DebugController : BaseController
{
    private readonly JwtConfiguration _jwtConfig;
    private readonly IDebugAccessService _debugAccessService;
    private readonly ITokenService _tokenService;
    private readonly ISecretProtector _secretProtector;
    private readonly ILogger<DebugController> _logger;

    public DebugController(
        IOptions<JwtConfiguration> jwtConfig,
        IDebugAccessService debugAccessService,
        ITokenService tokenService,
        ISecretProtector secretProtector,
        ILogger<DebugController> logger)
    {
        _jwtConfig = jwtConfig.Value;
        _debugAccessService = debugAccessService;
        _tokenService = tokenService;
        _secretProtector = secretProtector;
        _logger = logger;
    }

    [HttpPost("v1/generate-impersonate-link")]
    [ServiceFilter(typeof(DebugKeyAuthorizationFilter))]
    public async Task<IActionResult> GenerateImpersonateLink([FromBody] GenerateImpersonateLinkRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var url = await _debugAccessService.GenerateImpersonateLink(request.UserId, ip);

        return Ok(new { url });
    }

    [HttpPost("v1/generate-decrypt-link")]
    [ServiceFilter(typeof(DebugKeyAuthorizationFilter))]
    public async Task<IActionResult> GenerateDecryptLink([FromBody] GenerateDecryptLinkRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var url = await _debugAccessService.GenerateDecryptLink(request.ApiCredentialGuid, ip);

        return Ok(new { url });
    }

    [HttpGet("v1/impersonate")]
    public async Task<IActionResult> Impersonate([FromQuery] string token)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var debugToken = await _debugAccessService.ValidateAndConsumeToken(token, ip);

        if (debugToken.Purpose != DebugAccessPurpose.Impersonate || debugToken.User == null)
        {
            _logger.LogWarning("Invalid token purpose for impersonate. Expected: Impersonate, Got: {Purpose}", debugToken.Purpose);
            return BadRequest(new { error = "Invalid token purpose" });
        }

        var tokens = await _tokenService.GenerateTokens(debugToken.User, ip);

        // Set refresh token cookie (same as login)
        SetRefreshCookie(tokens.RefreshToken);

        _logger.LogWarning("DEBUG ACCESS: User {UserName} (ID: {UserId}) impersonated from IP {IP}",
            debugToken.User.UserName, debugToken.User.Id, ip);

        return Ok(new
        {
            access_token = tokens.AccessToken,
            refresh_token = tokens.RefreshToken,
            expires_in = (int)(tokens.AccessExpiresAt - DateTime.UtcNow).TotalSeconds,
            user = new
            {
                id = debugToken.User.Id,
                publicId = debugToken.User.PublicId,
                userName = debugToken.User.UserName,
                name = debugToken.User.Name
            }
        });
    }

    [HttpGet("v1/decrypt-credential")]
    public async Task<IActionResult> DecryptCredential([FromQuery] string token)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var debugToken = await _debugAccessService.ValidateAndConsumeToken(token, ip);

        if (debugToken.Purpose != DebugAccessPurpose.DecryptApiKey || debugToken.ApiCredential == null)
        {
            _logger.LogWarning("Invalid token purpose for decrypt. Expected: DecryptApiKey, Got: {Purpose}", debugToken.Purpose);
            return BadRequest(new { error = "Invalid token purpose" });
        }

        var decryptedToken = _secretProtector.Decrypt(debugToken.ApiCredential.TokenEncrypted);

        _logger.LogWarning("DEBUG ACCESS: API Credential {CredentialId} (Display: {DisplayName}) decrypted from IP {IP}",
            debugToken.ApiCredential.PublicId, debugToken.ApiCredential.DisplayName, ip);

        return Ok(new
        {
            credentialId = debugToken.ApiCredential.PublicId,
            displayName = debugToken.ApiCredential.DisplayName,
            userId = debugToken.ApiCredential.UserId,
            logicalAccountId = debugToken.ApiCredential.LogicalAccountId,
            apiEnvironment = debugToken.ApiCredential.ApiEnvironment.ToString(),
            decryptedToken
        });
    }

    private void SetRefreshCookie(string refreshToken)
    {
        Response.Cookies.Append(_jwtConfig.RefreshCookieName, refreshToken, new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtConfig.RefreshTokenDays)
        });
    }
}

public record GenerateImpersonateLinkRequest(long UserId);
public record GenerateDecryptLinkRequest(Guid ApiCredentialGuid);


