using Domain.Models.Responses;

namespace Domain.Repositories.Interfaces.VkOrd.Contract;

/// <summary>
/// Интерфейс для репозитория получения полных деталей контракта
/// </summary>
public interface IGetContractDetailsRepository
{
    /// <summary>
    /// Получает полные данные контракта с ассоциированными данными
    /// </summary>
    /// <param name="externalId">External ID контракта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>DTO с полными данными контракта или null если не найден</returns>
    Task<GetContractDetailsDto?> GetDetailsAsync(
        string externalId,
        CancellationToken cancellationToken);
}
