using Domain.Data;
using Domain.Entities.Enums;
using Domain.Entities.FlowTemplates.Requests;
using Domain.Entities.FlowTemplates.Responses;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

/// <summary>
/// Контроллер для управления шаблонами потоков
/// </summary>
[Route("api/flow-templates")]
[Authorize]
[ApiController]
public class FlowTemplatesController : BaseController
{
    private readonly IFlowTemplateService _service;

    public FlowTemplatesController(IFlowTemplateService service)
    {
        _service = service;
    }

    /// <summary>
    /// Создать новый шаблон потока
    /// </summary>
    [HttpPost("v1")]
    [ProducesResponseType(typeof(FlowTemplateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<FlowTemplateResponse> Create(
        [FromBody] CreateFlowTemplateRequest request,
        CancellationToken cancellationToken)
    {
        return _service.Create(request, cancellationToken);
    }

    /// <summary>
    /// Получить список шаблонов с фильтрацией
    /// </summary>
    [HttpGet("v1")]
    [ProducesResponseType(typeof(FlowTemplateListResponse), StatusCodes.Status200OK)]
    public Task<FlowTemplateListResponse> GetList(
        CancellationToken cancellationToken,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null,
        [FromQuery] FlowTemplateType? type = null,
        [FromQuery] string? tags = null,
        [FromQuery] string sort = "created_at",
        [FromQuery] string order = "desc",
        [FromQuery] bool activeOnly = false)
    {       
        // Парсим теги из comma-separated строки
        var tagsList = string.IsNullOrWhiteSpace(tags)
            ? null
            : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        return _service.GetList(
            cancellationToken,
            limit,
            offset,
            search,
            type,
            tagsList,
            sort,
            order,
            activeOnly);
    }

    /// <summary>
    /// Получить шаблон по ID
    /// </summary>
    [HttpGet("v1/{id:long}")]
    [ProducesResponseType(typeof(FlowTemplateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<FlowTemplateResponse> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        return _service.GetById(id, cancellationToken);
    }

    /// <summary>
    /// Обновить шаблон
    /// </summary>
    [HttpPut("v1/{id:long}")]
    [ProducesResponseType(typeof(FlowTemplateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<FlowTemplateResponse> Update(
        long id,
        [FromBody] UpdateFlowTemplateRequest request,
        CancellationToken cancellationToken)
    {
        return _service.Update(id, request, cancellationToken);
    }
    
    /// <summary>
    /// Обновить шаблон
    /// </summary>
    [HttpPut("v1/{id:long}/headers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task UpdateHeaders(
        long id,
        [FromBody] UpdateFlowTemplateHeadersRequest headersRequest,
        CancellationToken cancellationToken)
    {
        return _service.UpdateHeaders(id, headersRequest, cancellationToken);
    }

    /// <summary>
    /// Удалить шаблон (soft delete)
    /// </summary>
    [HttpDelete("v1/{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task Delete(
        long id,
        CancellationToken cancellationToken)
    {
        return _service.Delete(id, cancellationToken);
    }

    /// <summary>
    /// Активировать/деактивировать шаблон
    /// </summary>
    [HttpPatch("v1/{id:long}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task Activate(
        long id,
        [FromBody] ActivateFlowTemplateRequest request,
        CancellationToken cancellationToken)
    {
        return _service.Activate(id, request.IsActive, cancellationToken);
    }

    /// <summary>
    /// Увеличить счетчик использования шаблона
    /// </summary>
    [HttpPost("v1/{id:long}/use")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task IncrementUseCount(
        long id,
        CancellationToken cancellationToken)
    {
        return _service.IncrementUseCount(id, cancellationToken);
    }

    /// <summary>
    /// Получить все типы шаблонов
    /// </summary>
    [HttpGet("v1/types/all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FlowTemplateTypesResponse), StatusCodes.Status200OK)]
    public Task<FlowTemplateTypesResponse> GetTypes(
        CancellationToken cancellationToken)
    {
        return _service.GetTypes(cancellationToken);
    }
}
