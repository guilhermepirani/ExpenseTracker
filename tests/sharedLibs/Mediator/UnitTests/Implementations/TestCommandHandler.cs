using Mediator.Commands;

namespace Mediator.UnitTests;

/// <summary>
/// A test command handler that handles TestCommand{int}.
/// </summary>
public class IntCommandHandler : ICommandHandler<TestCommand<int>, int>
{
    public Task<int> HandleAsync(
        TestCommand<int> command,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(42);
    }
}

/// <summary>
/// A test command handler that handles TestCommand{string}.
/// </summary>
public class StringCommandHandler : ICommandHandler<TestCommand<string>, string>
{
    public Task<string> HandleAsync(
        TestCommand<string> command,
        CancellationToken cancellationToken)
    {
        return Task.FromResult("test result");
    }
}