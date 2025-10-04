using MediatR;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Features.Counterparties.Queries.GetCounterparty;

/// <summary>
/// Обработчик запроса получения контрагента VK ОРД.
/// </summary>
public class GetCounterpartyHandler : IRequestHandler<GetCounterpartyQuery, GetCounterpartyResponse>
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

    public async Task<GetCounterpartyResponse> Handle(GetCounterpartyQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCounterpartyQuery for {ExternalId}", request.ExternalId);

        return await _vkOrdService.GetCounterpartyByIdAsync(request.ExternalId, request.UserId, request.Environment);
    }
}

