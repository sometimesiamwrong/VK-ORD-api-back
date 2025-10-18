using Domain.VkOrdApi.Statistics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Requests;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers;

/// <summary>
/// Контроллер для работы со статистикой VK ORD
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IVkOrdService _vkOrdService;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(
        IVkOrdService vkOrdService,
        ILogger<StatisticsController> logger)
    {
        _vkOrdService = vkOrdService;
        _logger = logger;
    }

    /// <summary>
    /// Создать или обновить статистику
    /// POST /api/statistics
    /// </summary>
    [HttpPost("v1")]
    public async Task<IActionResult> CreateOrUpdateStatistics(
        [FromBody] CreateOrUpdateStatisticsRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating/Updating {Count} statistics", request.Items.Count);

        await _vkOrdService.CreateOrUpdateStatistics(request.Items, cancellationToken);

        return Ok(new { info = "Statistics created/updated successfully" });
    }

    /// <summary>
    /// Получить список статистик с фильтрацией и пагинацией
    /// GET /api/statistics
    /// </summary>
    [HttpGet("v1")]
    public async Task<IActionResult> GetStatisticsList(
        [FromQuery(Name = "creative_external_id")] string? creativeExternalId = null,
        [FromQuery(Name = "pad_external_id")] string? padExternalId = null,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting statistics list. CreativeExternalId: {CreativeExternalId}, " +
            "PadExternalId: {PadExternalId}, Offset: {Offset}, Limit: {Limit}",
            creativeExternalId, padExternalId, offset, limit);

        var result = await _vkOrdService.GetStatisticsList(
            creativeExternalId,
            padExternalId,
            offset,
            limit,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Удалить статистику
    /// POST /api/statistics/delete
    /// </summary>
    [HttpPost("v1/delete")]
    public async Task<IActionResult> DeleteStatistics(
        [FromBody] DeleteStatisticsRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting {Count} statistics", request.Items.Count);

        await _vkOrdService.DeleteStatistics(request.Items, cancellationToken);

        return Ok(new { info = "Statistics deleted successfully" });
    }
}
