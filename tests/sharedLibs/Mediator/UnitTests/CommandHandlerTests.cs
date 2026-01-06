using Mediator.Commands;
using Mediator.Dispatcher;
using Mediator.Pipelines;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Mediator.UnitTests;

/// <summary>
/// Tests for command dispatch functionality.
/// </summary>
public class CommandHandlerTests
{
    [Fact]
    public async Task WithValidCommand_ReturnsCorrectResult()
    {
        // Note: Behaviors are explicitly configured as empty (no pipeline behaviors in this test).
        // This is intentional as we're testing basic command dispatch without interceptors.
        // The mock builder handles GetServices(IPipelineBehaviour<,>) to satisfy the dispatcher contract.

        // Arrange
        const int expectedResult = 42;
        var command = new TestCommand<int>();
        var handlerMock = CommandHandlerMockBuilder
            .CreateMockCommandHandler<TestCommand<int>, int>(expectedResult);
        var serviceProviderMock = CommandHandlerMockBuilder
            .CreateMockServiceProviderWithCommandHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.HandleAsync(
            command,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedResult, result);
        handlerMock.Verify(h => h.HandleAsync(
            command,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WithStringResult_ReturnsCorrectResult()
    {
        // Arrange
        const string expectedResult = "success";
        var command = new TestCommand<string>();
        var handlerMock = CommandHandlerMockBuilder
            .CreateMockCommandHandler<TestCommand<string>, string>(expectedResult);
        var serviceProviderMock = CommandHandlerMockBuilder
            .CreateMockServiceProviderWithCommandHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.HandleAsync(
            command,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedResult, result);
        handlerMock.Verify(h => h.HandleAsync(
            command,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WithComplexType_ReturnsCorrectResult()
    {
        // Arrange
        var expectedResult = new { Id = 1, Name = "Test" };
        var command = new TestCommand<object>();
        var handlerMock = new Mock<ICommandHandler<TestCommand<object>, object>>();
        handlerMock.Setup(h => h.HandleAsync(
                It.IsAny<TestCommand<object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(typeof(TestCommand<object>), typeof(object));
        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<object>), typeof(object));
        var behaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(behaviourType);

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

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.HandleAsync(
            command,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task HandlerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new TestCommand<int>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));

        serviceProviderMock
            .Setup(sp => sp.GetService(handlerType))
            .Returns((object?)null);

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.HandleAsync(
                command,
                TestContext.Current.CancellationToken));
        Assert.Contains("No service", exception.Message);
    }

    [Fact]
    public async Task HandlerThrowsException_ExceptionPropagated()
    {
        // Arrange
        var command = new TestCommand<int>();
        var expectedException = new InvalidOperationException("Handler error");
        var handlerMock = new Mock<ICommandHandler<TestCommand<int>, int>>();
        handlerMock
            .Setup(h => h.HandleAsync(
                It.IsAny<TestCommand<int>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(behaviourType);

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

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.HandleAsync(
                command,
                TestContext.Current.CancellationToken));
        Assert.Equal("Handler error", exception.Message);
    }

    [Fact]
    public async Task CancellationTokenPropagation_TokenPassedToHandler()
    {
        // Arrange
        var command = new TestCommand<int>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var handlerMock = CommandHandlerMockBuilder
            .CreateMockCommandHandler<TestCommand<int>, int>(42);
        var serviceProviderMock = CommandHandlerMockBuilder
            .CreateMockServiceProviderWithCommandHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        await dispatcher.HandleAsync(command, cancellationToken);

        // Assert
        handlerMock.Verify(
            h => h.HandleAsync(command, cancellationToken),
            Times.Once,
            "Cancellation token should be passed to the handler");
    }

    [Fact]
    public async Task WithPreCancelledToken_ThrowsTaskCanceledException()
    {
        // Arrange
        var command = new TestCommand<int>();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        var cancellationToken = cancellationTokenSource.Token;

        var handlerMock = new Mock<ICommandHandler<TestCommand<int>, int>>();
        handlerMock
            .Setup(h => h.HandleAsync(
                It.IsAny<TestCommand<int>>(),
                It.IsAny<CancellationToken>()))
            .Returns((TestCommand<int> _, CancellationToken ct) =>
            {
                if (ct.IsCancellationRequested)
                    return Task.FromCanceled<int>(ct);
                return Task.FromResult(42);
            });

        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(behaviourType);

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

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TaskCanceledException>(
            () => dispatcher.HandleAsync(command, cancellationToken));
        Assert.IsType<TaskCanceledException>(exception);
    }

    [Fact]
    public async Task WithDefaultCancellationToken_UsesDefault()
    {
        // Arrange
        var command = new TestCommand<int>();
        var handlerMock = CommandHandlerMockBuilder
            .CreateMockCommandHandler<TestCommand<int>, int>(42);
        var serviceProviderMock = CommandHandlerMockBuilder
            .CreateMockServiceProviderWithCommandHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        await dispatcher.HandleAsync(
            command,
            TestContext.Current.CancellationToken);

        // Assert
        handlerMock.Verify(
            h => h.HandleAsync(command, TestContext.Current.CancellationToken),
            Times.Once);
    }
}