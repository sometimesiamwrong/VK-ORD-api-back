namespace Domain.Entities.Enums;

/// <summary>
/// Типы шаблонов потоков
/// </summary>
public enum FlowTemplateType
{
    /// <summary>Базовый шаблон потока</summary>
    Basic = 0,
    
    /// <summary>Шаблон для контрактов VK ORD</summary>
    VkOrdContract = 1,
    
    /// <summary>Шаблон для креативов</summary>
    VkOrdCreative = 2,
    
    /// <summary>Шаблон для статистики</summary>
    VkOrdStatistics = 3,

    /// <summary>Шаблон для Wizard</summary>
    VkOrdWizard = 4,
    
    /// <summary>Пользовательский шаблон</summary>
    Custom = 99
}
