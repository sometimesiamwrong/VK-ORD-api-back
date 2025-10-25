namespace Domain.Entities.FlowTemplates.Responses;

public class FlowTemplateListResponse
{
    public List<FlowTemplateListItemResponse> Data { get; set; } = new();
    public PaginationMetadata Pagination { get; set; } = new();
}

public class PaginationMetadata
{
    public int Total { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public bool HasMore => (Offset + Limit) < Total;
}
