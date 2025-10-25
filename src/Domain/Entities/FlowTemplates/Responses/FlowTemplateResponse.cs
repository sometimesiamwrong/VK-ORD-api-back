using Domain.Entities.Enums;

namespace Domain.Entities.FlowTemplates.Responses;

public class FlowTemplateResponse
{
    public long Id { get; set; }
    public Guid PublicId { get; set; }
    public long ApiCredentialId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FlowTemplateType Type { get; set; }
    public string? Description { get; set; }
    public object Value { get; set; } = new { };
    public List<string> Tags { get; set; } = new();
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
    public int UseCount { get; set; }
}
