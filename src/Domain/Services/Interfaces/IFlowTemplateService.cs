using Domain.Entities.Enums;
using Domain.Entities.FlowTemplates.Requests;
using Domain.Entities.FlowTemplates.Responses;

namespace Domain.Services.Interfaces;

/// <summary>
/// Сервис для работы с шаблонами потоков
/// </summary>
public interface IFlowTemplateService
{
    /// <summary>
    /// Создать новый шаблон потока
    /// </summary>
    Task<FlowTemplateResponse> Create(CreateFlowTemplateRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Получить шаблон по ID
    /// </summary>
    Task<FlowTemplateResponse> GetById(long templateId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список шаблонов с фильтрацией
    /// </summary>
    Task<FlowTemplateListResponse> GetList(
        CancellationToken cancellationToken,
        int limit = 50,
        int offset = 0,
        string? search = null,
        FlowTemplateType? type = null,
        List<string>? tags = null,
        string sort = "created_at",
        string order = "desc",
        bool activeOnly = false);

    /// <summary>
    /// Обновить шаблон
    /// </summary>
    Task<FlowTemplateResponse> Update(long templateId, UpdateFlowTemplateRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Обновить шаблон
    /// </summary>
    Task UpdateHeaders(long templateId, UpdateFlowTemplateHeadersRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить шаблон (soft delete)
    /// </summary>
    Task<bool> Delete(long templateId, CancellationToken cancellationToken);

    /// <summary>
    /// Активировать/деактивировать шаблон
    /// </summary>
    Task<bool> Activate(long templateId, bool isActive, CancellationToken cancellationToken);

    /// <summary>
    /// Увеличить счетчик использования шаблона
    /// </summary>
    Task<bool> IncrementUseCount(long templateId, CancellationToken cancellationToken);

    /// <summary>
    /// Получить список всех типов шаблонов
    /// </summary>
    Task<FlowTemplateTypesResponse> GetTypes(CancellationToken cancellationToken);
}
