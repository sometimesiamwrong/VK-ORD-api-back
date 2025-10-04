namespace WebApp.Models.Requests;

public abstract class IRequestWithVkOrdKey
{
    public Guid ApiCredentialPublicId { get; set; }
}
