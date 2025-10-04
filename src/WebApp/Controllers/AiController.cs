using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[Authorize]
public class AiController : BaseController
{
    private readonly IAiService _service;

    public AiController(IAiService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить классификацию KKTY по тексту
    /// </summary>
    [HttpPost("get-kkty_by-text")]
    public async Task<GetKktyByTextResponse> GetKktyByText([FromBody] GetKktyByTextRequest request, CancellationToken cancellationToken)
    {
        return await _service.GetKktyByTextAsync(request.Text, cancellationToken);
    }
}
