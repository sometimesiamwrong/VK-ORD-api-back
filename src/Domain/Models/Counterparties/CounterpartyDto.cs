using System.Text.Json.Serialization;

namespace Domain.Models.Counterparties;

/// <summary>
/// DTO контрагента
/// </summary>
public class CounterpartyDto
{
    /// <summary>
    /// Внешний идентификатор
    /// </summary>
    [JsonPropertyName("external_id")]
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// ИНН
    /// </summary>
    [JsonPropertyName("inn")]
    public string? Inn { get; set; }

    /// <summary>
    /// Название контрагента
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// URL сайта
    /// </summary>
    [JsonPropertyName("rs_url")]
    public string? RsUrl { get; set; }

    /// <summary>
    /// Роли контрагента
    /// </summary>
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Юридические детали
    /// </summary>
    [JsonPropertyName("juridical_details")]
    public JuridicalDetailsDto? JuridicalDetails { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    [JsonPropertyName("last_updated")]
    public DateTimeOffset? LastUpdated { get; set; }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    [JsonPropertyName("sync_status")]
    public string SyncStatus { get; set; } = "Synced";
}

/// <summary>
/// DTO юридических деталей
/// </summary>
public class JuridicalDetailsDto
{
    /// <summary>
    /// ИНН
    /// </summary>
    [JsonPropertyName("inn")]
    public string? Inn { get; set; }

    /// <summary>
    /// КПП
    /// </summary>
    [JsonPropertyName("kpp")]
    public string? Kpp { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Метод электронного платежа
    /// </summary>
    [JsonPropertyName("foreign_epayment_method")]
    public string? ForeignEpaymentMethod { get; set; }

    /// <summary>
    /// Регистрационный номер
    /// </summary>
    [JsonPropertyName("foreign_registration_number")]
    public string? ForeignRegistrationNumber { get; set; }

    /// <summary>
    /// ИНН иностранного контрагента
    /// </summary>
    [JsonPropertyName("foreign_inn")]
    public string? ForeignInn { get; set; }

    /// <summary>
    /// Код страны ОКСМ
    /// </summary>
    [JsonPropertyName("foreign_oksm_country_code")]
    public string? ForeignOksmCountryCode { get; set; }
}
