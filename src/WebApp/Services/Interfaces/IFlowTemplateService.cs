using Domain.Entities.Enums;
using Domain.Entities.FlowTemplates.Requests;
using Domain.Entities.FlowTemplates.Responses;

namespace WebApp.Services.Interfaces;

/// <summary>
/// Сервис для работы с шаблонами потоков
/// </summary>
public interface IFlowTemplateService
{
    /// <summary>
    /// Создать новый шаблон потока
    /// </summary>
    Task<FlowTemplateResponse> CreateAsync(CreateFlowTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить шаблон по ID
    /// </summary>
    Task<FlowTemplateResponse> GetByIdAsync(long templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список шаблонов с фильтрацией
    /// </summary>
    Task<FlowTemplateListResponse> GetListAsync(
        int limit = 50,
        int offset = 0,
        string? search = null,
        FlowTemplateType? type = null,
        List<string>? tags = null,
        string sort = "created_at",
        string order = "desc",
        bool activeOnly = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить шаблон
    /// </summary>
    Task<FlowTemplateResponse> UpdateAsync(long templateId, UpdateFlowTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить шаблон (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(long templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Активировать/деактивировать шаблон
    /// </summary>
    Task<bool> ActivateAsync(long templateId, bool isActive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Увеличить счетчик использования шаблона
    /// </summary>
    Task<bool> IncrementUseCountAsync(long templateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список всех типов шаблонов
    /// </summary>
    Task<FlowTemplateTypesResponse> GetTypesAsync(CancellationToken cancellationToken = default);
}
