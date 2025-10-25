using Domain.Entities.Enums;

namespace Domain.Entities.FlowTemplates.Responses;

public class FlowTemplateListItemResponse
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FlowTemplateType Type { get; set; }
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
    public int UseCount { get; set; }
}
