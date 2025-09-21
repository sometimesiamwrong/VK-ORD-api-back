namespace VkOrdApiWrapper.Models.Responses
{
    /// <summary>
    /// Базовый класс для всех API ответов
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Успешно ли выполнено
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Время выполнения запроса
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Создать успешный ответ
        /// </summary>
        public static ApiResponse Ok(string message = "Success") => new()
        {
            Success = true,
            Message = message
        };

        /// <summary>
        /// Создать ошибочный ответ
        /// </summary>
        public static ApiResponse Error(string message) => new()
        {
            Success = false,
            Message = message
        };
    }

    /// <summary>
    /// Универсальный API ответ с данными
    /// </summary>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Данные
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Создать успешный ответ с данными
        /// </summary>
        public static ApiResponse<T> Ok(T data, string message = "Success") => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

        /// <summary>
        /// Создать ошибочный ответ с данными
        /// </summary>
        public static new ApiResponse<T> Error(string message) => new()
        {
            Success = false,
            Message = message
        };
    }
}
