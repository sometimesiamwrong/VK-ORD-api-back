using Domain.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApi.Creative;
using WebApp.Features.Counterparties.Queries.GetCounterparty;
using WebApp.Handlers.Requests;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ClientApiController : BaseController
    {
        private readonly IMediator _mediator;

        public ClientApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Поиск компании или ИП по ИНН (client api)
        /// </summary>
        [HttpPost("party")]
        public Task<DaDataPartyShortResponse> FindParty([FromBody] FindPartyByInnQuery query, CancellationToken cancellationToken)
        {
            return _mediator.Send(query, cancellationToken);
        }

        /// <summary>
        /// Создать контрагента в VK ОРД по ИНН (client api)
        /// </summary>
        [HttpPost("set-counterparty")]
        public Task CreateCounterparty([FromBody] CreateCounterpartyFromInnRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }

        /// <summary>
        /// Создать контракт в VK ОРД
        /// </summary>
        [HttpPost("create_contract")]
        public Task CreateContract([FromBody] CreateContractRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }

        /// <summary>
        /// Создать креатив в VK ОРД
        /// </summary>
        [HttpPost("create_creative")]
        public Task<VkOrdCreativeV3RequestResponse> CreateCreative([FromBody] CreateCreativeRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }

        /// <summary>
        /// Получить список всех контрагентов из VK ОРД с полными данными
        /// </summary>
        [HttpGet("counterparties")]
        public Task<GetCounterpartiesResponseDto> GetCounterparties([FromQuery] GetCounterpartiesRequest query, CancellationToken cancellationToken)
        {
            return _mediator.Send(query, cancellationToken);
        }

        /// <summary>
        /// Получить контрагента по external_id из VK ОРД
        /// </summary>
        [HttpGet("counterparties/{externalId}")]
        public Task<GetCounterpartyResponse> GetCounterparty(GetCounterpartyQuery query, CancellationToken cancellationToken)
        {
            return _mediator.Send(query, cancellationToken);
        }
	}
}

