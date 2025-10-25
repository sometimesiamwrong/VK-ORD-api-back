using System.Text.Json.Serialization;

namespace Domain;

public class PageRequest
{
    /// <summary>
    /// Количество элементов, которые необходимо пропустить в запросе
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = 0;

    private int _limit = 100;

    /// <summary>
    /// Количество элементов, которые необходимо получить за один запрос
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit 
    {
        get 
        {
            if(NeedAll)
            {
                return UnlimitedLimit;
            }
            return _limit;
        }
        set => _limit = value;
    }

    /// <summary>
    /// Нужно ли получить все элементы
    /// </summary>
    [JsonPropertyName("need_all")]
    public bool NeedAll { get; set; } = false;

    /// <summary>
    /// Значение используется для первого запроса, после заменяется на Limit из ответа
    /// </summary>
    [JsonIgnore]
    public int UnlimitedLimit { get; set; } = int.MaxValue;

    /// <summary>
    /// Номер страницы
    /// </summary>
    [JsonIgnore]
    public int Page => Offset == 0 ? 1 : (Offset / Limit) + 1;
}