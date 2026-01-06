using Mediator.Commands;

namespace Mediator.UnitTests;

/// <summary>
/// A generic test command implementation used for testing command dispatch functionality.
/// </summary>
/// <typeparam name="TResult">The type of result this command will produce.</typeparam>
public class TestCommand<TResult> : ICommand<TResult>
{
    /// <summary>
    /// Gets or sets an optional payload that can be used in testing scenarios.
    /// </summary>
    public object? Payload { get; set; }
}