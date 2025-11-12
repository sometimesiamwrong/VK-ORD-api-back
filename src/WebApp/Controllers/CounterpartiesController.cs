using Domain;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Features.Counterparties.Queries.GetCounterparty;
using Domain.Handlers.Requests;
using Domain.Models.Contracts;
using Domain.Models.Requests;
using Domain.Models.Responses;
using Domain.Services.Interfaces;
using Domain.VkOrdApi.Creative;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[Authorize]
public class CounterpartiesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IVkOrdService _vkOrdService;

    public CounterpartiesController(IMediator mediator, IVkOrdService vkOrdService)
    {
        _mediator = mediator;
        _vkOrdService = vkOrdService;
    }
    
    /// <summary>
    /// Создать контрагента в VK ОРД по ИНН
    /// </summary>
    [HttpPost("v1")]
    public Task CreateCounterparty([FromBody] CreateCounterpartyFromInnRequest request, CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    /// Получить список всех контрагентов из VK ОРД с полными данными
    /// </summary>
    [HttpGet("v1")]
    public Task<GetCounterpartiesResponseDto> GetCounterparties([FromQuery] GetCounterpartiesRequest query, CancellationToken cancellationToken)
    {
        return _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Получить контрагента по external_id из VK ОРД
    /// </summary>
    [HttpGet("v1/{externalId}")]
    public Task<VkOrdCounterparty> GetCounterparty([FromRoute] string externalId, CancellationToken cancellationToken)
    {
        return _mediator.Send(new GetCounterpartyQuery(externalId), cancellationToken);
    }

    /// <summary>
    /// Получить список всех договоров для контрагента из VK ОРД
    /// </summary>
    [HttpGet("v1/{externalId}/contracts")]
    public Task<CounterpartyWithContractsDto> GetContractsToCounterparty([FromRoute] string externalId, CancellationToken cancellationToken)
    {
        return _vkOrdService.GetContractsToCounterparty(externalId, cancellationToken);
    }
}