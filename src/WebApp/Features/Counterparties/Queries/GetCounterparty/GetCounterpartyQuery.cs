using MediatR;
using WebApp.Models.Responses;

namespace WebApp.Features.Counterparties.Queries.GetCounterparty;

/// <summary>
/// Запрос на получение контрагента VK ОРД.
/// </summary>
public record GetCounterpartyQuery(string ExternalId) : IRequest<GetCounterpartyResponse>;

