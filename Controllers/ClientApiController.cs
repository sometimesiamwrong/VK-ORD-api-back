using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Requests;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;

namespace VkOrdApiWrapper.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ClientController : BaseApiController
    {
        private readonly IDaDataService _daDataService;
        private readonly ILogger<ClientController> _logger;
        private readonly IVkOrdService _vkOrdService;

        public ClientController(IDaDataService daDataService, IVkOrdService vkOrdService, ILogger<ClientController> logger)
        {
            _daDataService = daDataService;
            _vkOrdService = vkOrdService;
            _logger = logger;
        }

        /// <summary>
        /// Поиск компании или ИП по ИНН (client api)
        /// </summary>
        [HttpPost("party")]
        public async Task<ApiResponse<DaDataPartyShortResponse>> FindParty([FromBody] FindPartyByInnRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<DaDataPartyShortResponse>("Некорректный ИНН");
            }

            var result = await _daDataService.FindPartyByInnAsync(request.Inn, cancellationToken);
            if (result is null)
            {
                HttpContext.Response.StatusCode = 404;
                return Error<DaDataPartyShortResponse>("Запись не найдена");
            }

            return Ok(result, "Найдена запись");
        }

        /// <summary>
        /// Создать контрагента в VK ОРД по ИНН (client api)
        /// </summary>
        [HttpPost("set-counterparty")]
        public async Task<ApiResponse> CreateCounterparty([FromBody] CreateCounterpartyFromInnRequest request)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return Error("Некорректный ИНН");
            }

            var result = await _vkOrdService.CreateCounterpartyFromInnAsync(request.Inn);
            if (result.Status == "success")
            {
                return Ok(result.Message);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return Error(result.Message);
            }
        }

        /// <summary>
        /// Создать контракт в VK ОРД
        /// </summary>
        [HttpPost("create_contract")]
        public async Task<ApiResponse<CreateContractResponse>> CreateContract([FromBody] CreateContractRequest request)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateContractResponse>("Invalid request data");
            }


            var result = await _vkOrdService.CreateOrUpdateContractAsync(request);
            if (result.Success)
            {
                return Ok(result, "Contract created successfully");
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateContractResponse>(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Создать креатив в VK ОРД
        /// </summary>
        [HttpPost("create_creative")]
        public async Task<ApiResponse<CreateCreativeResponse>> CreateCreative([FromBody] CreateCreativeRequest request)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateCreativeResponse>("Invalid request data");
            }

            var result = await _vkOrdService.CreateCreativeAsync(request);
            if (result.Success)
            {
                return Ok(result, "Creative created successfully");
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
                return Error<CreateCreativeResponse>(result.ErrorMessage);
            }
        }
	}
}

