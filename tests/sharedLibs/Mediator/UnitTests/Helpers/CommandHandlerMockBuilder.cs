using Mediator.Commands;
using Mediator.Pipelines;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Mediator.UnitTests;

/// <summary>
/// Helper class for creating mocked command handlers and service providers for testing.
/// 
/// This builder abstracts the pipeline behavior mocking logic, ensuring tests remain compatible
/// with the behavior pipeline contract implemented in CommandQueryDispatcher. The dispatcher
/// uses <c>GetServices(typeof(IPipelineBehaviour&lt;,&gt;))</c> to resolve behaviors, which this
/// builder properly mocks to prevent test fragility.
/// </summary>
public static class CommandHandlerMockBuilder
{
    /// <summary>
    /// Creates a mock service provider with a configured command handler.
    /// Behaviors are explicitly configured as an empty list (no pipeline behaviors).
    /// </summary>
    public static Mock<IServiceProvider> CreateMockServiceProviderWithCommandHandler<TCommand, TResult>(
        Mock<ICommandHandler<TCommand, TResult>> handlerMock)
        where TCommand : ICommand<TResult>
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        // Register using IRequestHandler base interface (what dispatcher looks for)
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResult));
        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TCommand), typeof(TResult));
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
    /// Creates a mock service provider with a configured command handler and specific behaviors.
    /// This method allows testing of pipeline behavior execution during command dispatch.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="handlerMock">The mocked command handler.</param>
    /// <param name="behaviors">The pipeline behaviors to configure.</param>
    /// <returns>A configured mock service provider.</returns>
    public static Mock<IServiceProvider> CreateMockServiceProviderWithCommandHandlerAndBehaviors<TCommand, TResult>(
        Mock<ICommandHandler<TCommand, TResult>> handlerMock,
        object[] behaviors)
        where TCommand : ICommand<TResult>
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        // Register using IRequestHandler base interface (what dispatcher looks for)
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResult));
        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TCommand), typeof(TResult));
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
    /// Creates a mock command handler that returns a specific result.
    /// </summary>
    public static Mock<ICommandHandler<TCommand, TResult>> CreateMockCommandHandler<TCommand, TResult>(
        TResult resultToReturn)
        where TCommand : ICommand<TResult>
    {
        var handlerMock = new Mock<ICommandHandler<TCommand, TResult>>();
        handlerMock
            .Setup(h => h.HandleAsync(It.IsAny<TCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultToReturn);

        return handlerMock;
    }
}