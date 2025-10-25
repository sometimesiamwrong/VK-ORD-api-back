using Domain.Entities.Enums;

namespace Domain.Entities.FlowTemplates.Responses;

public class FlowTemplateTypesResponse
{
    public List<FlowTemplateTypeDto> Types { get; set; } = new();
}

public class FlowTemplateTypeDto
{
    public FlowTemplateType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
