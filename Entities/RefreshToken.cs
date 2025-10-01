using System.ComponentModel.DataAnnotations;

namespace VkOrdApiWrapper.Entities
{
    /// <summary>
    /// Refresh токены с ротацией
    /// </summary>
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(64)]
        public string? CreatedByIp { get; set; }

        [MaxLength(128)]
        public string? DeviceId { get; set; }

        public DateTime? RevokedAt { get; set; }

        [MaxLength(256)]
        public string? ReplacedByTokenHash { get; set; }

        public bool IsRevoked => RevokedAt.HasValue;
    }
}


