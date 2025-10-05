using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Requests;

public class UpdateApiCredentialRequest
{
    public string? Environment { get; set; }

    public string? TokenPlain { get; set; }

    public string? DisplayName { get; set; }
}
