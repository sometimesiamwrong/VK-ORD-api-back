using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

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
		[HttpGet("party/{inn}")]
		public Task<DaDataPartyShortResponse?> FindPartyByInn(string inn, CancellationToken cancellationToken)
		{
			return _service.FindPartyByInnAsync(inn, cancellationToken);
		}
	}
}

