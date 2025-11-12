using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Requests;

public class CreateApiCredentialRequest
{
    [Required]
    public string Environment { get; set; } = string.Empty;

    [Required]
    public string TokenPlain { get; set; } = string.Empty;

    public string? DisplayName { get; set; }
}
