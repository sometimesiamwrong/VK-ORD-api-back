namespace VkOrdApiWrapper.Models.Requests;

public class GetKktyByTextRequest : AuthorizedRequestBase
{
    public string Text { get; set; } = string.Empty;
}
