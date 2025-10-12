using System.ComponentModel;

namespace Domain.Entities.Enums;

/// <summary>
/// Среда API
/// </summary>
public enum VkOrdApiEnvironmentCode
{
    /// <summary>
    /// Неизвестно
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Production
    /// </summary>
    [Description("https://api.ord.vk.com")]
    Production = 1,

    /// <summary>
    /// Sandbox
    /// </summary>
    [Description("https://api-sandbox.ord.vk.com")]
    Sandbox = 2,
}
