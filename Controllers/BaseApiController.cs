using Microsoft.AspNetCore.Mvc;
using VkOrdApiWrapper.Models.Responses;

namespace VkOrdApiWrapper.Controllers
{
    /// <summary>
    /// Базовый контроллер для автоматической обертки результатов в API ответы
    /// </summary>
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Возвращает успешный результат
        /// </summary>
        protected ApiResponse Ok() => ApiResponse.Ok();

        /// <summary>
        /// Возвращает успешный результат с сообщением
        /// </summary>
        protected ApiResponse Ok(string message) => ApiResponse.Ok(message);

        /// <summary>
        /// Возвращает успешный результат с данными
        /// </summary>
        protected ApiResponse<T> Ok<T>(T data, string message = "Success") =>
            ApiResponse<T>.Ok(data, message);

        /// <summary>
        /// Возвращает ошибку
        /// </summary>
        protected ApiResponse Error(string message) => ApiResponse.Error(message);

        /// <summary>
        /// Возвращает ошибку с данными
        /// </summary>
        protected ApiResponse<T> Error<T>(string message) => ApiResponse<T>.Error(message);

        /// <summary>
        /// Устанавливает результат для middleware
        /// </summary>
        protected void SetApiResponse(object response)
        {
            HttpContext.Items["ApiResponse"] = response;
        }

        /// <summary>
        /// Переопределяем стандартные методы для автоматической обертки
        /// </summary>
        public override OkObjectResult Ok(object? value)
        {
            if (value is ApiResponse apiResponse)
            {
                SetApiResponse(apiResponse);
                return base.Ok(apiResponse);
            }
            return base.Ok(value);
        }

        public override BadRequestObjectResult BadRequest(object? error)
        {
            if (error is ApiResponse apiResponse)
            {
                SetApiResponse(apiResponse);
                return base.BadRequest(apiResponse);
            }
            return base.BadRequest(error);
        }

        public override NotFoundObjectResult NotFound(object? value)
        {
            if (value is ApiResponse apiResponse)
            {
                SetApiResponse(apiResponse);
                return base.NotFound(apiResponse);
            }
            return base.NotFound(value);
        }
    }
}
