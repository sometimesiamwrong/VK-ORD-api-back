using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Responses;
using VkOrdApiWrapper.Services.Interfaces;

namespace VkOrdApiWrapper.Controllers
{
	[Route("api/[controller]")]
	[Authorize]
	public class DaDataController : BaseApiController
	{
		private readonly IDaDataService _service;
		private readonly ILogger<DaDataController> _logger;

		public DaDataController(IDaDataService service, ILogger<DaDataController> logger)
		{
			_service = service;
			_logger = logger;
		}

		/// <summary>
		/// Поиск компании или ИП по ИНН
		/// </summary>
		[HttpGet("party/{inn}")]
		public async Task<ApiResponse<DaDataPartyShortResponse?>> FindPartyByInn(string inn, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(inn))
			{
				HttpContext.Response.StatusCode = 400;
				return Error<DaDataPartyShortResponse?>("ИНН не указан");
			}

			var result = await _service.FindPartyByInnAsync(inn, cancellationToken);
			if (result is null)
			{
				HttpContext.Response.StatusCode = 404;
				return Error<DaDataPartyShortResponse?>("Запись не найдена");
			}

			return Ok(result, "Найдена запись");
		}
	}
}

