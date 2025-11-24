using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Enums;

namespace Domain.Entities;

[Table("DebugAccessTokens")]
public class DebugAccessToken : EntityBase
{
    public string Token { get; set; } = null!;

    public DebugAccessPurpose Purpose { get; set; }

    public long? UserId { get; set; }

    public long? ApiCredentialId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    public string? CreatedByIp { get; set; }

    public string? UsedByIp { get; set; }

    public virtual User? User { get; set; }

    public virtual ApiCredential? ApiCredential { get; set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public bool IsValid => !IsUsed && !IsExpired && !IsDeleted;
}
