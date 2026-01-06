using Mediator.Queries;

namespace Mediator.UnitTests;

/// <summary>
/// A test query handler that handles TestQuery{int}.
/// </summary>
public class IntQueryHandler : IQueryHandler<TestQuery<int>, int>
{
    public Task<int> HandleAsync(
        TestQuery<int> query,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(100);
    }
}

/// <summary>
/// A test query handler that handles TestQuery{string}.
/// </summary>
public class StringQueryHandler : IQueryHandler<TestQuery<string>, string>
{
    public Task<string> HandleAsync(
        TestQuery<string> query,
        CancellationToken cancellationToken)
    {
        return Task.FromResult("query result");
    }
}