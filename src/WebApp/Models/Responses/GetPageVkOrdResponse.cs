using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Responses
{
    public class GetPageVkOrdResponse
    {
        /// <summary>
        /// Внешние ID элементов    
        /// </summary>
        public List<string> ExternalIds { get; set; } = new();

        /// <summary>
        /// Общее количество элементов в VK ORD
        /// </summary>
        public int TotalCount => ExternalIds?.Count ?? 0;

        /// <summary>
        /// Общее количество элементов в VK ORD
        /// </summary>
        public int TotalItemsCount { get; set; } // Общее количество элементов в VK ORD

        /// <summary>
        /// Лимит элементов за запрос
        /// </summary>
        public int Limit { get; set; } // Лимит элементов за запрос
    }
}