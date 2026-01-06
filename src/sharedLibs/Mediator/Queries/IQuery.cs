namespace Mediator.Queries;

public interface IQuery<out T> : IRequest<T>;