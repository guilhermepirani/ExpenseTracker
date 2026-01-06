using Mediator.Pipelines;
using Mediator.Queries;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Mediator.UnitTests;

/// <summary>
/// Helper class for creating mocked query handlers and service providers for testing.
/// 
/// This builder abstracts the pipeline behavior mocking logic, ensuring tests remain compatible
/// with the behavior pipeline contract implemented in CommandQueryDispatcher. The dispatcher
/// uses <c>GetServices(typeof(IPipelineBehaviour&lt;,&gt;))</c> to resolve behaviors, which this
/// builder properly mocks to prevent test fragility.
/// </summary>
public static class QueryHandlerMockBuilder
{
    /// <summary>
    /// Creates a mock service provider with a configured query handler.
    /// Behaviors are explicitly configured as an empty list (no pipeline behaviors).
    /// </summary>
    public static Mock<IServiceProvider> CreateMockServiceProviderWithQueryHandler<TQuery, TResult>(
        Mock<IQueryHandler<TQuery, TResult>> handlerMock)
        where TQuery : IQuery<TResult>
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        // Register using IRequestHandler base interface (what dispatcher looks for)
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResult));
        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TQuery), typeof(TResult));
        var behaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(behaviourType);

        // Single setup with callback handles all service types
        serviceProviderMock
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns((Type serviceType) =>
            {
                if (serviceType == handlerType)
                    return handlerMock.Object;
                if (serviceType == behaviourEnumerableType)
                    return Enumerable.Empty<object>();
                return null;
            });

        return serviceProviderMock;
    }

    /// <summary>
    /// Creates a mock service provider with a configured query handler and specific behaviors.
    /// This method allows testing of pipeline behavior execution during query dispatch.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="handlerMock">The mocked query handler.</param>
    /// <param name="behaviors">The pipeline behaviors to configure.</param>
    /// <returns>A configured mock service provider.</returns>
    public static Mock<IServiceProvider> CreateMockServiceProviderWithQueryHandlerAndBehaviors<TQuery, TResult>(
        Mock<IQueryHandler<TQuery, TResult>> handlerMock,
        object[] behaviors)
        where TQuery : IQuery<TResult>
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        // Register using IRequestHandler base interface (what dispatcher looks for)
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResult));
        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TQuery), typeof(TResult));
        var behaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(behaviourType);

        serviceProviderMock
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns((Type serviceType) =>
            {
                if (serviceType == handlerType)
                    return handlerMock.Object;
                if (serviceType == behaviourEnumerableType)
                    return behaviors.Length > 0 ? behaviors : Enumerable.Empty<object>();
                return null;
            });

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