using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Handlers.Requests;
using WebApp.Models.Responses;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : BaseController
    {
        private readonly IMediator _mediator;

        public ClientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Поиск компании или ИП по ИНН (DaData API)
        /// </summary>
        [HttpPost("v1/party")]
        public Task<DaDataPartyShortResponse> FindParty([FromBody] FindPartyByInnQuery query, CancellationToken cancellationToken)
        {
            return _mediator.Send(query, cancellationToken);
        }
	}
}

