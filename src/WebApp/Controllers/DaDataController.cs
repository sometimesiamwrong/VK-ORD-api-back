using Domain.Models.Responses;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	[Route("api/[controller]")]
	[Authorize]
	public class DaDataController : BaseController
	{
		private readonly IDaDataService _service;

		public DaDataController(IDaDataService service)
		{
			_service = service;
		}

		/// <summary>
		/// Поиск компании или ИП по ИНН
		/// </summary>
		[HttpGet("v1/party/{inn}")]
		public Task<DaDataPartyShortResponse?> FindPartyByInn(string inn, CancellationToken cancellationToken)
		{
			return _service.FindPartyByInnAsync(inn, cancellationToken);
		}
	}
}

