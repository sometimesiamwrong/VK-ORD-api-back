using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Requests;

public class UpdateUserRequest
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    [MaxLength(200)]
    public required string Name { get; set; }
}
