using Mediator.Queries;

using Moq;

namespace Mediator.UnitTests;

/// <summary>
/// Helper class for creating mocked query handlers and service providers for testing.
/// </summary>
public static class QueryHandlerMockBuilder
{
    /// <summary>
    /// Creates a mock service provider with a configured query handler.
    /// </summary>
    public static Mock<IServiceProvider> CreateMockServiceProviderWithQueryHandler<TQuery, TResult>(
        Mock<IQueryHandler<TQuery, TResult>> handlerMock)
        where TQuery : IQuery<TResult>
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResult));

        serviceProviderMock
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handlerMock.Object);

        return serviceProviderMock;
    }

    /// <summary>
    /// Creates a mock query handler that returns a specific result.
    /// </summary>
    public static Mock<IQueryHandler<TQuery, TResult>> CreateMockQueryHandler<TQuery, TResult>(
        TResult resultToReturn)
        where TQuery : IQuery<TResult>
    {
        var handlerMock = new Mock<IQueryHandler<TQuery, TResult>>();
        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultToReturn);

        return handlerMock;
    }
}