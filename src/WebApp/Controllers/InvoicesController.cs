using Domain;
using Domain.Entities.VkOrd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Requests;
using WebApp.Models.Responses;
using WebApp.Services.Interfaces;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[Authorize]
public class InvoicesController : BaseController
{
    private readonly IVkOrdService _vkOrdService;

    public InvoicesController(IVkOrdService vkOrdService)
    {
        _vkOrdService = vkOrdService;
    }

    /// <summary>
    /// Создать или обновить акт в VK ОРД
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные акта</param>
    /// <param name="draft">Является ли черновиком (по умолчанию false)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPut("{externalId}")]
    public Task CreateOrUpdateInvoice(
        string externalId,
        [FromBody] CreateOrUpdateInvoiceRequest request,
        [FromQuery] bool draft = false,
        CancellationToken cancellationToken = default)
    {
        // Устанавливаем externalId из маршрута
        request.ExternalId = externalId;

        return _vkOrdService.CreateOrUpdateInvoice(request, draft, cancellationToken);
    }

    /// <summary>
    /// Получить информацию об акте по external_id
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpGet("{externalId}")]
    public Task<VkOrdInvoice> GetInvoice(string externalId, CancellationToken cancellationToken)
    {
        return _vkOrdService.GetInvoice(externalId, cancellationToken);
    }

    /// <summary>
    /// Получить список актов с пагинацией
    /// </summary>
    /// <param name="pageRequest">Параметры пагинации</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpGet]
    public Task<GetInvoicesDto> GetInvoices(
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        return _vkOrdService.GetPageInvoice(pageRequest, cancellationToken);
    }

    /// <summary>
    /// Удалить акт из VK ОРД
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpDelete("{externalId}")]
    public Task DeleteInvoice(string externalId, CancellationToken cancellationToken)
    {
        return _vkOrdService.DeleteInvoice(externalId, cancellationToken);
    }

    /// <summary>
    /// Добавить договоры в акт
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные договоров для добавления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPatch("{externalId}/items")]
    public Task AddContractsToInvoice(
        string externalId,
        [FromBody] Domain.VkOrdApi.Invoice.VkOrdApiAddContractsToInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        return _vkOrdService.AddContractsToInvoice(externalId, request, cancellationToken);
    }

    /// <summary>
    /// Удалить договоры из акта
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные договоров для удаления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPost("{externalId}/delete")]
    public Task DeleteContractsFromInvoice(
        string externalId,
        [FromBody] Domain.VkOrdApi.Invoice.VkOrdApiDeleteContractsFromInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        return _vkOrdService.DeleteContractsFromInvoice(externalId, request, cancellationToken);
    }

    /// <summary>
    /// Отправить акт в ЕРИР
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPost("{externalId}/ready")]
    public Task SendInvoiceToErir(string externalId, CancellationToken cancellationToken)
    {
        return _vkOrdService.SendInvoiceToErir(externalId, cancellationToken);
    }

    /// <summary>
    /// Создать или обновить заголовок акта (без разаллокаций)
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="request">Данные заголовка акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpPut("{externalId}/header")]
    public Task CreateOrUpdateInvoiceHeader(
        string externalId,
        [FromBody] Domain.VkOrdApi.Invoice.VkOrdApiInvoiceHeaderRequest request,
        CancellationToken cancellationToken)
    {
        return _vkOrdService.CreateOrUpdateInvoiceHeader(externalId, request, cancellationToken);
    }

    /// <summary>
    /// Получить заголовок акта (без разаллокаций)
    /// </summary>
    /// <param name="externalId">Внешний идентификатор акта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    [HttpGet("{externalId}/header")]
    public Task<Domain.VkOrdApi.Invoice.VkOrdApiInvoiceHeaderResponse> GetInvoiceHeader(
        string externalId,
        CancellationToken cancellationToken)
    {
        return _vkOrdService.GetInvoiceHeader(externalId, cancellationToken);
    }
}
