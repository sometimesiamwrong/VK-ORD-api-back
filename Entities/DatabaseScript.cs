using System.ComponentModel.DataAnnotations;

namespace VkOrdApiWrapper.Entities
{
    /// <summary>
    /// Сущность для отслеживания выполненных SQL-скриптов
    /// </summary>
    public class DatabaseScript
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string ScriptName { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string ScriptHash { get; set; } = string.Empty;

        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsSuccessful { get; set; } = true;

        [MaxLength(2000)]
        public string? ErrorMessage { get; set; }
    }
}
