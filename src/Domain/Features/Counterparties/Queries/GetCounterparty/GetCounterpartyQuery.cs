using Domain.Entities.VkOrd;
using MediatR;

namespace Domain.Features.Counterparties.Queries.GetCounterparty;

/// <summary>
/// Запрос на получение контрагента VK ОРД.
/// </summary>
public record GetCounterpartyQuery(string ExternalId) : IRequest<VkOrdCounterparty>;

