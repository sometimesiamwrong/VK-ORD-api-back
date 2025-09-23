using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;

namespace VkOrdApiWrapper.Controllers;

[Route("api/ai")]
[Authorize]
public class AiController : BaseApiController
{
    private readonly IAiService _service;
    private readonly ILogger<AiController> _logger;

    public AiController(IAiService service, ILogger<AiController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Получить классификацию KKTY по тексту
    /// </summary>
    [HttpPost("get-kkty_by-text")]
    public async Task<ApiResponse<GetKktyByTextResponse>> GetKktyByText([FromBody] GetKktyByTextRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            HttpContext.Response.StatusCode = 400;
            return Error<GetKktyByTextResponse>("Текст для классификации не указан");
        }

        try
        {
            var result = await _service.GetKktyByTextAsync(request.Text, cancellationToken);
            return Ok(result, "Классификация KKTY получена");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting KKTY classification for text: {Text}", request.Text);
            HttpContext.Response.StatusCode = 500;
            return Error<GetKktyByTextResponse>("Ошибка при получении классификации KKTY");
        }
    }
}
