using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VkOrdApiWrapper.Entities
{
    /// <summary>
    /// VK ORD учетные данные пользователя
    /// </summary>
    public class ApiCredential
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Environment { get; set; } = "Sandbox"; // Sandbox|Production

        [Required]
        public string TokenEncrypted { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? DisplayName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int RowVersion { get; set; }
    }
}


