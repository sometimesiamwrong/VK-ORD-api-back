using System.Text.Json.Serialization;
using Domain.Entities.Enums.VkOrd;

namespace Domain.VkOrdApi.Person;

public sealed class VkOrdApiPersonJuridicalDetails
{
    /// <summary>
    /// Тип юридического лица
    /// </summary>
    [JsonPropertyName("type")]
    public VkOrdApiPersonType Type { get; set; }

    /// <summary>
    /// Схема модели
    /// </summary>
    [JsonPropertyName("model_scheme")]
    public string ModelScheme { get; set; } = "russia";

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