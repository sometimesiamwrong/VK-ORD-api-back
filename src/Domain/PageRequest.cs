using System.Text.Json.Serialization;

namespace Domain;

public class PageRequest
{
    /// <summary>
    /// Количество элементов, которые необходимо пропустить в запросе
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Количество элементов, которые необходимо получить за один запрос
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 100;
}