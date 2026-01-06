using Mediator.Queries;

namespace Mediator.UnitTests;

/// <summary>
/// A generic test query implementation used for testing query dispatch functionality.
/// </summary>
/// <typeparam name="TResult">The type of result this query will produce.</typeparam>
public class TestQuery<TResult> : IQuery<TResult>
{
    /// <summary>
    /// Gets or sets an optional payload that can be used in testing scenarios.
    /// </summary>
    public object? Payload { get; set; }
}