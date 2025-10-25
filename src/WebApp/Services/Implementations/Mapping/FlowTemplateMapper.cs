using Domain.Entities;
using Domain.Entities.FlowTemplates.Responses;
using System.Text.Json;

namespace WebApp.Services.Implementations.Mapping;

/// <summary>
/// Маппер для FlowTemplate сущностей
/// </summary>
public static class FlowTemplateMapper
{
    /// <summary>
    /// Маппинг FlowTemplate в FlowTemplateResponse
    /// </summary>
    public static FlowTemplateResponse ToResponse(FlowTemplate template, object data)
    {
        return new FlowTemplateResponse
        {
            Id = template.Id,
            PublicId = template.PublicId,
            ApiCredentialId = template.ApiCredentialId,
            Name = template.Name,
            Type = template.Type,
            Description = template.Description,
            Value = data,
            Tags = DeserializeTags(template.Tags),
            Version = template.Version,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            LastUsedAt = template.LastUsedAt,
            UseCount = template.UseCount
        };
    }

    /// <summary>
    /// Маппинг FlowTemplate в FlowTemplateListItemResponse
    /// </summary>
    public static FlowTemplateListItemResponse ToListItemResponse(FlowTemplate template)
    {
        return new FlowTemplateListItemResponse
        {
            Id = template.Id,
            PublicId = template.PublicId,
            Name = template.Name,
            Type = template.Type,
            Description = template.Description,
            Tags = DeserializeTags(template.Tags),
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            LastUsedAt = template.LastUsedAt,
            UseCount = template.UseCount
        };
    }

    /// <summary>
    /// Десериализация JSON Tags в List of string
    /// </summary>
    private static List<string> DeserializeTags(string? tagsJson)
    {
        if (string.IsNullOrWhiteSpace(tagsJson))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(tagsJson) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
