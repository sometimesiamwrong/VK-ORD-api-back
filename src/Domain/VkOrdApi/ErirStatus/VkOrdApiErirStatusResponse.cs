using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.ErirStatus;

/// <summary>
/// Ответ со статусом обработки объекта в ЕРИР (GET /v1/{data_type}/{external_id}/erir_status).
/// Содержит текущий статус обработки, timestamps и ошибки если применимо.
/// </summary>
public sealed class VkOrdApiErirStatusResponse
{
    /// <summary>
    /// Статус обработки объекта по работе с рекламой от ЕРИР.
    /// Возможные значения: processing (в обработке), bad (не прошёл проверку), verified (проверку пройдено успешно).
    /// </summary>
    [JsonPropertyName("erir_status")]
    public VkOrdApiErirStatus ApiErirStatus { get; set; }

    /// <summary>
    /// Дата и время последнего обновления объекта по работе с рекламой в формате ISO 8601.
    /// Выставляется когда erir_status = processing.
    /// Пример: 2023-05-25T12:17:26Z
    /// </summary>
    [JsonPropertyName("updated_by_user_ts")]
    public string UpdatedByUserTs { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время окончания обработки объекта по работе с рекламой в формате ISO 8601.
    /// Выставляется когда erir_status = bad или verified. Nullable.
    /// Пример: 2023-05-28T12:17:26Z
    /// </summary>
    [JsonPropertyName("finalized_ts")]
    public string? FinalizedTs { get; set; }

    /// <summary>
    /// Список ошибок о статусе обработки от ЕРИР.
    /// Возвращается если erir_status = bad. Nullable.
    /// Пример: ["Invoice start period date must be less than or equal statistics start date"]
    /// </summary>
    [JsonPropertyName("messages")]
    public List<string>? Messages { get; set; }
}
