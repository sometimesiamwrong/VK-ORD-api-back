using Domain.Entities.Enums;

namespace Domain.Entities.FlowTemplates.Requests;

public class CreateFlowTemplateRequest
{
    public required string Name { get; set; }
    public required FlowTemplateType Type { get; set; }
    public string? Description { get; set; }
    public required object Value { get; set; }
    public List<string>? Tags { get; set; }
}
