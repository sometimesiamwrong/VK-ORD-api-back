using Domain.BrokenRules;
using Domain.Entities.VkOrd;
using Domain.Extensions;
using Domain.Services.Interfaces;
using MediatR;

namespace Domain.Features.Counterparties.Queries.GetCounterparty;

/// <summary>
/// Обработчик запроса получения контрагента VK ОРД.
/// </summary>
public class GetCounterpartyHandler : IRequestHandler<GetCounterpartyQuery, VkOrdCounterparty>
{
    private readonly IVkOrdService _vkOrdService;
    private readonly ILogger<GetCounterpartyHandler> _logger;

    public GetCounterpartyHandler(
        IVkOrdService vkOrdService,
        ILogger<GetCounterpartyHandler> logger)
    {
        _vkOrdService = vkOrdService;
        _logger = logger;
    }

    public async Task<VkOrdCounterparty> Handle(GetCounterpartyQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCounterpartyQuery for {ExternalId}", request.ExternalId);

        var result = await _vkOrdService.GetCounterpartyById(request.ExternalId, cancellationToken);

        if (result == null)
        {
            throw BrokenRuleCodes.CounterpartyNotFound.AsExn();
        }

        return result;
    }
}

