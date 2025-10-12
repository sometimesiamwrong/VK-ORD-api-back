using Domain.VkOrdApi.Creative;

namespace WebApp.Models.Responses
{
    /// <summary>
    /// Ответ при получении списка креативов с полными данными
    /// </summary>
    public class GetCreativesResponse
    {
        public List<VkOrdApiCreativeV3Response> Data { get; set; } = new();
        public int TotalCount => Data?.Count ?? 0;
        public int TotalItemsCount { get; set; }
        public int Limit { get; set; }
    }
}




