using System.Text.Json.Serialization;

namespace Domain.VkOrdApi.Invoice;

/// <summary>
/// Запрос для отправки акта в ЕРИР (POST /v2/invoice/{external_id}/ready)
/// Тело запроса пустое согласно Swagger
/// </summary>
public sealed class VkOrdApiSendInvoiceToErirRequest
{
    // Пустой класс - API не требует параметров в теле запроса
}
