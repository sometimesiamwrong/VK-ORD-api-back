using System.Security.Authentication;
using Domain.BrokenRules;
using Domain.Extensions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Configuration;
using WebApp.Models.Responses;
using WebApp.Security;
using WebApp.Handlers.Requests;

namespace WebApp.Controllers; 

[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly JwtConfiguration _jwtConfig;
    private readonly IMediator _mediator;

    public AuthController(
        IOptions<JwtConfiguration> jwtConfig, 
        IMediator mediator)
    {
        _jwtConfig = jwtConfig.Value;
        _mediator = mediator;
    }

    /// <summary>
    /// Получить JWT токен для доступа к API
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<AuthResponse> Register([FromBody] RegisterRequest request)
    {
        var command = new WebApp.Handlers.Requests.RegisterUserCommand 
        { 
            UserName = request.UserName, 
            Password = request.Password, 
            Name = request.Name, 
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString() 
        };

        var tokens = await _mediator.Send(command);
        SetRefreshCookie(tokens.RefreshToken);
        var authResponse = AuthResponse.Create(tokens.AccessToken, _jwtConfig.ExpiryInMinutes * 60, tokens.RefreshToken);

        return authResponse;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var query = new LoginUserQuery 
        { 
            UserName = request.UserName, 
            Password = request.Password, 
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString() 
        };

        var tokens = await _mediator.Send(query);
        SetRefreshCookie(tokens.RefreshToken);
        var authResponse = AuthResponse.Create(tokens.AccessToken, _jwtConfig.ExpiryInMinutes * 60, tokens.RefreshToken);

        return authResponse;
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<AuthResponse> Refresh()
    {
        var refresh = Request.Cookies[_jwtConfig.RefreshCookieName];
        if (string.IsNullOrEmpty(refresh))
        {
            throw BrokenRuleCodes.UserNotAuthorized.AsExn();
        }

        var command = new RefreshTokenCommand 
        { 
            RefreshToken = refresh,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        var newTokens = await _mediator.Send(command);
        SetRefreshCookie(newTokens.RefreshToken);
        var authResponse = AuthResponse.Create(newTokens.AccessToken, _jwtConfig.ExpiryInMinutes * 60, newTokens.RefreshToken);
        return authResponse;
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var refresh = Request.Cookies[_jwtConfig.RefreshCookieName];
        if (!string.IsNullOrEmpty(refresh))
        {
            var hash = TokenService.ComputeSha256(refresh);
            var command = new RevokeRefreshTokenCommand { TokenHash = hash };
            await _mediator.Send(command);
        }

        Response.Cookies.Delete(_jwtConfig.RefreshCookieName, new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        return Ok();
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

/// <summary>
/// Запрос на получение токена
/// </summary>
public class LoginRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}

public class RegisterRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
}
