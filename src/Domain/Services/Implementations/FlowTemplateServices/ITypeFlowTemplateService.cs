namespace Domain.Services.Implementations.FlowTemplateServices
{
    /// <summary>
    /// Сервис для работы с шаблонами потоков
    /// </summary>
    public interface ITypeFlowTemplateService
    {
        /// <summary>
        /// Десериализовать запрос
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        Task<object> GetData(string value, CancellationToken cancellationToken);

        /// <summary>
        /// Проверить запрос
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        Task CheckRequest(string value, CancellationToken cancellationToken);
    }
}