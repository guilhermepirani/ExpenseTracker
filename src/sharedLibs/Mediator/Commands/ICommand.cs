namespace Mediator.Commands;

public interface ICommand<out T> : IRequest<T>;