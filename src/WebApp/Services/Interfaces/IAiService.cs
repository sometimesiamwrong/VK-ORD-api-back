using WebApp.Models.Responses;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Сервис для работы с AI
/// </summary>
public interface IAiService
{
    Task<GetKktyByTextResponse> GetKktyByTextAsync(string text, CancellationToken cancellationToken);
}
