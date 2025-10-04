using MediatR;
using WebApp.Models.Responses;

namespace WebApp.Handlers.Requests
{
    public record FindPartyByInnQuery(string Inn) : IRequest<DaDataPartyShortResponse>;
}
