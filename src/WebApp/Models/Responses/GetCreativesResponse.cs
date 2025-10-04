namespace WebApp.Models.Responses
{
    /// <summary>
    /// Ответ при получении списка креативов с полными данными
    /// </summary>
    public class GetCreativesResponse
    {
        public bool Success { get; set; }
        public List<VkOrdCreative> Creatives { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public int TotalCount => Creatives?.Count ?? 0;
        public int TotalItemsCount { get; set; }
        public int Limit { get; set; }
    }
}




