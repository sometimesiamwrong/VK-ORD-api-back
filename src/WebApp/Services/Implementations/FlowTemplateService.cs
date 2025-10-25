using Domain.BrokenRules;
using Domain.Data;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Entities.FlowTemplates.Requests;
using Domain.Entities.FlowTemplates.Responses;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApp.Services.Implementations.FlowTemplateServices;
using WebApp.Services.Implementations.Mapping;
using WebApp.Services.Interfaces;

namespace WebApp.Services.Implementations;

/// <summary>
/// Сервис для работы с шаблонами потоков
/// </summary>
public class FlowTemplateService : IFlowTemplateService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FlowTemplateService> _logger;
    private readonly IVkOrdApiClientFactory _vkOrdApiClientFactory;
    private readonly IWizardFlowTemplateService _wizardFlowTemplateService;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public FlowTemplateService(
        AppDbContext context,
        ILogger<FlowTemplateService> logger,
        IVkOrdApiClientFactory vkOrdApiClientFactory,
        IWizardFlowTemplateService wizardFlowTemplateService,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _context = context;
        _logger = logger;
        _vkOrdApiClientFactory = vkOrdApiClientFactory;
        _wizardFlowTemplateService = wizardFlowTemplateService;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <inheritdoc />
    public async Task<FlowTemplateResponse> CreateAsync(
        CreateFlowTemplateRequest request, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var apiCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();

        // Проверяем уникальность имени для данного ApiCredential
        var exists = await _context.FlowTemplates
            .AnyAsync(t => t.ApiCredentialId == apiCredential.Id 
                          && t.Name == request.Name 
                          && !t.IsDeleted, 
                cancellationToken);

        if (exists)
        {
            throw BrokenRuleCodes.FlowTemplateWithSuchNameAlreadyExists.AsExn();
        }

        var flowTemplateService = GetFlowTemplate(request.Type);

        // Сериализуем Value в JSON
        await flowTemplateService.CheckRequest(request.Value.ToString(), cancellationToken);
        
        var tagsJson = request.Tags != null && request.Tags.Any() 
            ? JsonSerializer.Serialize(request.Tags) 
            : null;

        var template = new FlowTemplate
        {
            ApiCredentialId = apiCredential.Id,
            Name = request.Name,
            Type = request.Type,
            Description = request.Description,
            Value = JsonSerializer.Serialize(request.Value, _jsonSerializerOptions),
            Tags = tagsJson,
            PublicId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            IsActive = true,
            UseCount = 0,
            Version = 1
        };

        _context.FlowTemplates.Add(template);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created flow template {TemplateId} with name '{Name}' for ApiCredential {ApiCredentialId}",
            template.Id, template.Name, apiCredential.Id);

        return FlowTemplateMapper.ToResponse(template, request.Value);
    }

    /// <inheritdoc />
    public async Task<FlowTemplateResponse?> GetByIdAsync(
        long templateId, 
        CancellationToken cancellationToken = default)
    {
        var apiCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();

        var template = await _context.FlowTemplates
            .Where(t => t.Id == templateId 
                       && t.ApiCredentialId == apiCredential.Id 
                       && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (template == null)
        {
            _logger.LogWarning(
                "Flow template {TemplateId} not found or access denied for ApiCredential {ApiCredentialId}",
                templateId, apiCredential.Id);
            return null;
        }

        var flowTemplateService = GetFlowTemplate(template.Type);
        var data = await flowTemplateService.GetData(template.Value, cancellationToken);

        _logger.LogDebug(
            "Retrieved flow template {TemplateId} for ApiCredential {ApiCredentialId}",
            templateId, apiCredential.Id);

        var response = FlowTemplateMapper.ToResponse(template, data);
        return response;
    }

    /// <inheritdoc />
    public async Task<FlowTemplateListResponse> GetListAsync(
        int limit = 50,
        int offset = 0,
        string? search = null,
        FlowTemplateType? type = null,
        List<string>? tags = null,
        string sort = "created_at",
        string order = "desc",
        bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var apiCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();
        var query = _context.FlowTemplates
            .Where(t => t.ApiCredentialId == apiCredential.Id && !t.IsDeleted);

        // Фильтр по активности
        if (activeOnly)
        {
            query = query.Where(t => t.IsActive);
        }

        // Фильтр по типу
        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        // Фильтр по поиску в имени и описании
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(t => 
                t.Name.ToLower().Contains(searchLower) || 
                (t.Description != null && t.Description.ToLower().Contains(searchLower)));
        }

        // Фильтр по тегам (если теги есть в JSON)
        if (tags != null && tags.Any())
        {
            foreach (var tag in tags)
            {
                query = query.Where(t => t.Tags != null && t.Tags.Contains(tag));
            }
        }

        // Подсчет общего количества
        var total = await query.CountAsync(cancellationToken);

        // Сортировка
        query = sort.ToLower() switch
        {
            "name" => order.ToLower() == "asc" 
                ? query.OrderBy(t => t.Name) 
                : query.OrderByDescending(t => t.Name),
            "use_count" => order.ToLower() == "asc" 
                ? query.OrderBy(t => t.UseCount) 
                : query.OrderByDescending(t => t.UseCount),
            "last_used_at" => order.ToLower() == "asc" 
                ? query.OrderBy(t => t.LastUsedAt) 
                : query.OrderByDescending(t => t.LastUsedAt),
            _ => order.ToLower() == "asc" 
                ? query.OrderBy(t => t.CreatedAt) 
                : query.OrderByDescending(t => t.CreatedAt)
        };

        // Пагинация
        var templates = await query
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        _logger.LogDebug(
            "Retrieved {Count} flow templates for ApiCredential {ApiCredentialId} (offset: {Offset}, limit: {Limit})",
            templates.Count, apiCredential.Id, offset, limit);

        return new FlowTemplateListResponse
        {
            Data = templates.Select(FlowTemplateMapper.ToListItemResponse).ToList(),
            Pagination = new PaginationMetadata
            {
                Total = total,
                Limit = limit,
                Offset = offset
            }
        };
    }

    /// <inheritdoc />
    public async Task<FlowTemplateResponse> UpdateAsync(
        long templateId, 
        UpdateFlowTemplateRequest request, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var apiCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();

        var template = await _context.FlowTemplates
            .Where(t => t.Id == templateId 
                       && t.ApiCredentialId == apiCredential.Id 
                       && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (template == null)
        {
            _logger.LogWarning(
                "Flow template {TemplateId} not found or access denied for ApiCredential {ApiCredentialId}",
                templateId, apiCredential.Id);
            throw BrokenRuleCodes.FlowTemplateNotFound.AsExn();
        }

        // Проверяем уникальность имени, если имя изменяется
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != template.Name)
        {
            var exists = await _context.FlowTemplates
                .AnyAsync(t => t.ApiCredentialId == apiCredential.Id 
                              && t.Name == request.Name 
                              && t.Id != templateId
                              && !t.IsDeleted, 
                    cancellationToken);

            if (exists)
            {
                throw BrokenRuleCodes.FlowTemplateWithSuchNameAlreadyExists.AsExn();
            }

            template.Name = request.Name;
        }
        var flowTemplateService = GetFlowTemplate(request.Type);
        await flowTemplateService.CheckRequest(request.Value.ToString(), cancellationToken);

        template.Type = request.Type;
        template.Description = request.Description;
        template.Value = JsonSerializer.Serialize(request.Value, _jsonSerializerOptions);

        if (!request.Tags.IsNullOrEmpty())
        {
            template.Tags = JsonSerializer.Serialize(request.Tags, _jsonSerializerOptions);
        }

        template.IsActive = request.IsActive;

        // Инкрементируем версию и обновляем дату
        template.Version++;
        template.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated flow template {TemplateId} (version {Version}) for ApiCredential {ApiCredentialId}",
            template.Id, template.Version, apiCredential.Id);

        return FlowTemplateMapper.ToResponse(template, request.Value);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        long templateId, 
        CancellationToken cancellationToken = default)
    {
        var apiCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();

        var template = await _context.FlowTemplates
            .Where(t => t.Id == templateId 
                       && t.ApiCredentialId == apiCredential.Id 
                       && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (template == null)
        {
            _logger.LogWarning(
                "Flow template {TemplateId} not found or access denied for ApiCredential {ApiCredentialId}",
                templateId, apiCredential.Id);
            return false;
        }

        // Soft delete
        template.IsDeleted = true;
        template.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Deleted (soft) flow template {TemplateId} for ApiCredential {ApiCredentialId}",
            templateId, apiCredential.Id);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ActivateAsync(
        long templateId, 
        bool isActive, 
        CancellationToken cancellationToken = default)
    {
        var apiCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();
        var template = await _context.FlowTemplates
            .Where(t => t.Id == templateId 
                       && t.ApiCredentialId == apiCredential.Id 
                       && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (template == null)
        {
            _logger.LogWarning(
                "Flow template {TemplateId} not found or access denied for ApiCredential {ApiCredentialId}",
                templateId, apiCredential.Id);
            return false;
        }

        template.IsActive = isActive;
        template.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "{Action} flow template {TemplateId} for ApiCredential {ApiCredentialId}",
            isActive ? "Activated" : "Deactivated", templateId, apiCredential.Id);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> IncrementUseCountAsync(
        long templateId, 
        CancellationToken cancellationToken = default)
    {
        var apiCredential = await _vkOrdApiClientFactory.GetVkOrdCredentialAsync();

        var template = await _context.FlowTemplates
            .Where(t => t.Id == templateId 
                       && t.ApiCredentialId == apiCredential.Id 
                       && !t.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (template == null)
        {
            _logger.LogWarning(
                "Flow template {TemplateId} not found or access denied for ApiCredential {ApiCredentialId}",
                templateId, apiCredential.Id);
            return false;
        }

        template.UseCount++;
        template.LastUsedAt = DateTimeOffset.UtcNow;
        template.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Incremented use count for flow template {TemplateId} (count: {UseCount}) for ApiCredential {ApiCredentialId}",
            templateId, template.UseCount, apiCredential.Id);

        return true;
    }

    /// <inheritdoc />
    public async Task<FlowTemplateTypesResponse> GetTypesAsync(CancellationToken cancellationToken = default)
    {
        // Получаем все значения enum
        var types = Enum.GetValues<FlowTemplateType>()
            .Select(t => new FlowTemplateTypeDto
            {
                Type = t,
                Name = t.ToString(),
                Description = GetTypeDescription(t)
            })
            .ToList();

        var response = new FlowTemplateTypesResponse
        {
            Types = types
        };

        _logger.LogDebug("Retrieved and cached {Count} flow template types", types.Count);

        return response;
    }

    /// <summary>
    /// Получить описание типа шаблона
    /// </summary>
    private static string GetTypeDescription(FlowTemplateType type)
    {
        return type switch
        {
            FlowTemplateType.Basic => "Базовый шаблон потока",
            FlowTemplateType.VkOrdContract => "Шаблон для контрактов VK ORD",
            FlowTemplateType.VkOrdCreative => "Шаблон для креативов VK ORD",
            FlowTemplateType.VkOrdStatistics => "Шаблон для статистики VK ORD",
            FlowTemplateType.VkOrdWizard => "Шаблон для Wizard",
            FlowTemplateType.Custom => "Пользовательский шаблон",
            _ => type.ToString()
        };
    }

    private ITypeFlowTemplateService GetFlowTemplate(FlowTemplateType type)
    {
        return type switch{
            FlowTemplateType.VkOrdWizard => _wizardFlowTemplateService,
            _ => throw new InvalidOperationException($"Неизвестный тип шаблона: {type}")
        };
    }
}
