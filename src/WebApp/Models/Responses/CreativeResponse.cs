namespace WebApp.Models.Responses
{
    /// <summary>
    /// Ответ при получении информации о креативе
    /// </summary>
    public class CreativeResponse
    {
        /// <summary>
        /// Данные креатива
        /// </summary>
        public VkOrdCreative Data { get; set; } = new();

        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; } = string.Empty;

        /// <summary>
        /// Статус креатива
        /// </summary>
        public VkOrdStatusResponse Status { get; set; } = new();

        /// <summary>
        /// Создать ответ из ответа VK ОРД
        /// </summary>
        public static CreativeResponse FromVkOrdResponse(VkOrdErrorResponse<VkOrdCreative> vkOrdResponse, string externalId)
        {
            if (vkOrdResponse.IsSuccess)
            {
                return new CreativeResponse
                {
                    Data = vkOrdResponse.Data,
                    ExternalId = externalId
                };
            }
            else
            {
                return new CreativeResponse
                {
                    ExternalId = externalId
                };
            }
        }
    }
    
    /// <summary>
    /// Ответ при получении статуса креатива
    /// </summary>
    public class CreativeStatusResponse
    {
        /// <summary>
        /// Данные статуса
        /// </summary>
        public VkOrdStatusResponse Data { get; set; } = new();

        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; } = string.Empty;

        /// <summary>
        /// Создать ответ при получении статуса креатива
        /// </summary>
        public static CreativeStatusResponse Create(string externalId, VkOrdStatusResponse status)
        {
            return new CreativeStatusResponse
            {
                Data = status,
                ExternalId = externalId
            };
        }
    }

    /// <summary>
    /// Ответ при проверке верификации креатива
    /// </summary>
    public class CreativeVerificationResponse
    {
        /// <summary>
        /// Результат верификации
        /// </summary>
        public bool Data { get; set; }

        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; } = string.Empty;

        /// <summary>
        /// Время проверки
        /// </summary>
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Создать ответ при проверке верификации креатива
        /// </summary>
        public static CreativeVerificationResponse Create(string externalId, bool isVerified)
        {
            return new CreativeVerificationResponse
            {
                Data = isVerified,
                ExternalId = externalId,
                CheckedAt = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Ответ при пакетном создании креативов
    /// </summary>
    public class BulkCreativeResponse
    {
        /// <summary>
        /// Результаты создания креативов
        /// </summary>
        public List<CreateCreativeResponse> Data { get; set; } = new();

        /// <summary>
        /// Общее количество запрошенных креативов
        /// </summary>
        public int TotalRequested { get; set; }

        /// <summary>
        /// Общее количество созданных креативов
        /// </summary>
        public int TotalCreated { get; set; }

        /// <summary>
        /// Общее количество неудачных креативов
        /// </summary>
        public int TotalFailed { get; set; }

        /// <summary>
        /// Ошибки
        /// </summary>
        public List<string> Errors { get; set; } = new();

        public static BulkCreativeResponse Create(List<CreateCreativeResponse> results, int requestedCount)
        {
            var response = new BulkCreativeResponse
            {
                Data = results,
                TotalRequested = requestedCount,
                TotalCreated = results.Count(r => r.Success),
                TotalFailed = results.Count(r => !r.Success)
            };

            // Собираем ошибки
            var failedResults = results.Where(r => !r.Success && !string.IsNullOrEmpty(r.ErrorMessage));
            response.Errors.AddRange(failedResults.Select(r => $"{r.ExternalId}: {r.ErrorMessage}"));

            return response;
        }
    }
}
