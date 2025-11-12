namespace Domain.Handlers
{
    /// <summary>
    /// Базовый интерфейс для всех хендлеров
    /// </summary>
    public interface IHandler<TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
