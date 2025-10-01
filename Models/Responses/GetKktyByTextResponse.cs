using System.Text.Json.Serialization;

namespace VkOrdApiWrapper.Models.Responses;

public class GetKktyByTextResponse
{
    public List<KktyItem> KKTY { get; set; } = new();
    public List<MatchedCategory> MatchedCategories { get; set; } = new();
}

public class KktyItem
{
    public string Code { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
}

public class MatchedCategory
{
    public string MainCategoryId { get; set; } = string.Empty;
    public string MainCategoryName { get; set; } = string.Empty;
    public string SubcategoryId { get; set; } = string.Empty;
    public string SubcategoryName { get; set; } = string.Empty;
    public List<string> MatchedItemsInSubcategory { get; set; } = new();
}
