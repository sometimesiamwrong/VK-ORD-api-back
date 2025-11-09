using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.ErirStatus;

/// <summary>
/// Запрос на получение статусов нескольких объектов рекламы в ЕРИР (POST /v1/erir_statuses).
/// Содержит фильтры для выборки статусов обработки.
/// </summary>
public sealed class VkOrdApiErirStatusesRequest
{
    /// <summary>
    /// Объекты по работе с рекламой, по которым выполняется фильтрация выдачи.
    /// Возможные значения: person (контрагенты), contract (договоры), creative (креативы),
    /// pad (рекламные площадки), invoice (акты), statistics (статистика), cid (Contract ID).
    /// Required.
    /// Пример: person
    /// </summary>
    [JsonPropertyName("data_type")]
    [Required]
    public VkOrdApiErirDataType? DataType { get; set; }

    /// <summary>
    /// Статусы обработки от ЕРИР, по которым выполняется фильтрация выдачи.
    /// Возможные значения: processing (в обработке), bad (не прошёл проверку), verified (проверку пройдено успешно).
    /// Optional.
    /// Пример: bad
    /// </summary>
    [JsonPropertyName("erir_status")]
    public VkOrdApiErirStatus? ErirStatus { get; set; }

    /// <summary>
    /// Количество элементов выдачи, которые необходимо пропустить в запросе.
    /// Значение по умолчанию: 0.
    /// Пример: 1
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Количество всех элементов, которые необходимо получить за один запрос.
    /// Значение по умолчанию: 10000. Максимальное: 60000.
    /// Тесно связано с limit_per_entity.
    /// Пример: 1000
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 10000;

    /// <summary>
    /// Максимальное количество элементов каждого объекта по работе с рекламой за один запрос.
    /// Тесно связано с limit. Порядок объектов: person, contract, creative, invoice, pad, statistics.
    /// Примеры: Если limit=10 и 9 person — вернёт 9 person + 1 contract.
    /// Если limit_per_entity=3 — вернёт по 3 на тип.
    /// Если limit=10, limit_per_entity=3 — 3 person, 3 contract, 3 creative, 1 invoice.
    /// Optional.
    /// Пример: 1
    /// </summary>
    [JsonPropertyName("limit_per_entity")]
    public int? LimitPerEntity { get; set; }

    /// <summary>
    /// Список внешних идентификаторов объектов по работе с рекламой, статусы которых необходимо получить.
    /// Optional.
    /// Пример: ["uphvmeo04lo-1h4iv1hdd", "qr7kai25pag-1h43euct5"]
    /// </summary>
    [JsonPropertyName("external_id")]
    public List<string>? ExternalIds { get; set; }
}
