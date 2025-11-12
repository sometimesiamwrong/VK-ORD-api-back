using Domain.Models.Responses;
using MediatR;

namespace Domain.Handlers.Requests
{
    public record FindPartyByInnQuery(string Inn) : IRequest<DaDataPartyShortResponse>;
}
