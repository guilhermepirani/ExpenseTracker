namespace Mediator.Queries;

public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query,
        CancellationToken cancellationToken = default);
}