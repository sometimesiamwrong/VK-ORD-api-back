using VkOrdApiWrapper.Models.VkOrd;

namespace VkOrdApiWrapper.Models.Responses
{
    /// <summary>
    /// Ответ при получении информации о креативе
    /// </summary>
    public class CreativeResponse : ApiResponse<VkOrdCreative>
    {
        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Статус креатива
        /// </summary>
        public VkOrdStatusResponse Status { get; set; }
        
        /// <summary>
        /// Создать ответ из ответа VK ОРД
        /// </summary>
        public static CreativeResponse FromVkOrdResponse(VkOrdResponse<VkOrdCreative> vkOrdResponse, string externalId)
        {
            if (vkOrdResponse.IsSuccess)
            {
                return new CreativeResponse
                {
                    Success = true,
                    Message = "Creative found",
                    Data = vkOrdResponse.Data,
                    ExternalId = externalId
                };
            }
            else
            {
                return new CreativeResponse
                {
                    Success = false,
                    Message = vkOrdResponse.Error ?? "Creative not found",
                    ExternalId = externalId
                };
            }
        }
    }
    
    /// <summary>
    /// Ответ при получении статуса креатива
    /// </summary>
    public class CreativeStatusResponse : ApiResponse<VkOrdStatusResponse>
    {
        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Создать ответ при получении статуса креатива
        /// </summary>
        public static CreativeStatusResponse Create(string externalId, VkOrdStatusResponse status)
        {
            return new CreativeStatusResponse
            {
                Success = true,
                Message = "Status retrieved successfully",
                Data = status,
                ExternalId = externalId
            };
        }
    }

    /// <summary>
    /// Ответ при проверке верификации креатива
    /// </summary>
    public class CreativeVerificationResponse : ApiResponse<bool>
    {
        /// <summary>
        /// Внешний ID креатива
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Время проверки
        /// </summary>
        public DateTime CheckedAt { get; set; }

        /// <summary>
        /// Создать ответ при проверке верификации креатива
        /// </summary>
        public static CreativeVerificationResponse Create(string externalId, bool isVerified)
        {
            return new CreativeVerificationResponse
            {
                Success = true,
                Message = isVerified ? "Creative is verified" : "Creative is not verified yet",
                Data = isVerified,
                ExternalId = externalId,
                CheckedAt = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Ответ при пакетном создании креативов
    /// </summary>
    public class BulkCreativeResponse : ApiResponse<List<CreateCreativeResponse>>
    {
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
                Success = true,
                Message = $"Bulk operation completed. Created: {results.Count(r => r.Success)}, Failed: {results.Count(r => !r.Success)}",
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
