using VkOrdApiWrapper.Models.Responses;

namespace VkOrdApiWrapper.Services.Interfaces;

public interface IAiService
{
    Task<GetKktyByTextResponse> GetKktyByTextAsync(string text, CancellationToken cancellationToken = default);
}
