using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace Domain.Entities.VkOrd
{
    /// <summary>
    /// Суммаризирующая сущность разных ключей и доступов для одного VK ORD аккаунта
    /// </summary>
    public class VkOrdLogicalAccount : EntityBase
    {
        /// <summary>
        /// Учетные данные
        /// </summary>
        public virtual ICollection<VkOrdBase> VkOrdEntities { get; set; } = new List<VkOrdBase>();
    }
}