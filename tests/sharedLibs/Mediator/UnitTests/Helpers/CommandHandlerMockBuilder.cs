using Mediator.Commands;

using Moq;

namespace Mediator.UnitTests;

/// <summary>
/// Helper class for creating mocked command handlers and service providers for testing.
/// </summary>
public static class CommandHandlerMockBuilder
{
    /// <summary>
    /// Creates a mock service provider with a configured command handler.
    /// </summary>
    public static Mock<IServiceProvider> CreateMockServiceProviderWithCommandHandler<TCommand, TResult>(
        Mock<ICommandHandler<TCommand, TResult>> handlerMock)
        where TCommand : ICommand<TResult>
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResult));

        serviceProviderMock
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handlerMock.Object);

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