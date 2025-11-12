namespace Domain.Models.Responses;

public class ApiCredentialResponse
{
    public Guid PublicId { get; set; }

    public string Environment { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
