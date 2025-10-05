namespace WebApp.Models.Responses;

public class UserProfileResponse
{
    /// <summary>
    /// Внешний ID пользователя
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Флаг активности пользователя
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Дата создания пользователя
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления пользователя
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
