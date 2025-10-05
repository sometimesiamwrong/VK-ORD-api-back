using MediatR;

namespace Domain;

    /// <summary>
    /// Базовый интерфейс команды
    /// </summary>
    /// <typeparam name="TRequest">тип запроса</typeparam>
    /// <typeparam name="TResponse">тип ответа</typeparam>
    public interface IHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse> 
    {
        /// <summary>
        /// Обработать команду
        /// </summary>
        /// <param name="request">Команда на обработку</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        new Task<TResponse> Handle(TRequest request,
                                   CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TRequest,TResponse> : IHandler<TRequest, TResponse> where TRequest : IRequest<TResponse> { }
    
    /// <summary>
    /// Базовый интерфейс команды без возвращения результата
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    public interface ISimpleCommandHandler<in TRequest> : ICommandHandler<TRequest, Unit>  where TRequest : IRequest<Unit> { }
    
    /// <summary>
    /// Контракт для команды.
    /// </summary>
    /// <typeparam name="T">Тип результата выполнения команды.</typeparam>
    public interface ICommand<out T> : IRequest<T> { }