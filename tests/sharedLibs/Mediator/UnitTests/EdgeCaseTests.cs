using Mediator.Commands;
using Mediator.Dispatcher;
using Mediator.Pipelines;
using Mediator.Queries;

using Moq;

namespace Mediator.UnitTests;

/// <summary>
/// Tests for edge cases and integration scenarios.
/// </summary>
public class EdgeCaseTestsTests
{
    [Fact]
    public async Task WithNullServiceProvider_ThrowsException()
    {
        // Arrange
        var command = new TestCommand<int>();
        IServiceProvider nullServiceProvider = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => new CommandQueryDispatcher(nullServiceProvider)
                .HandleAsync(command, TestContext.Current.CancellationToken));
        Assert.Contains("provider", exception.Message);
    }

    [Fact]
    public async Task CommandAndQueryDifferentHandlers_BothResolved()
    {
        // Arrange
        var command = new TestCommand<int>();
        var query = new TestQuery<int>();
        var commandHandlerMock = CommandHandlerMockBuilder
            .CreateMockCommandHandler<TestCommand<int>, int>(42);
        var queryHandlerMock = QueryHandlerMockBuilder
            .CreateMockQueryHandler<TestQuery<int>, int>(100);

        var serviceProviderMock = new Mock<IServiceProvider>();

        var commandHandlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var queryHandlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));
        var commandBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var queryBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));
        var commandBehaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(commandBehaviourType);
        var queryBehaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(queryBehaviourType);

        serviceProviderMock
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns((Type serviceType) =>
            {
                if (serviceType == commandHandlerType)
                    return commandHandlerMock.Object;
                if (serviceType == queryHandlerType)
                    return queryHandlerMock.Object;
                if (serviceType == commandBehaviourEnumerableType || serviceType == queryBehaviourEnumerableType)
                    return Enumerable.Empty<object>();
                return null;
            });

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var commandResult = await dispatcher.HandleAsync(
            command,
            TestContext.Current.CancellationToken);
        var queryResult = await dispatcher.HandleAsync(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, commandResult);
        Assert.Equal(100, queryResult);
        commandHandlerMock.Verify(h => h.HandleAsync(
            command,
            It.IsAny<CancellationToken>()),
            Times.Once);
        queryHandlerMock.Verify(h => h.HandleAsync(
            query,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MultipleCommandsWithDifferentTypes_EachReturnCorrectType()
    {
        // Arrange
        var intCommand = new TestCommand<int>();
        var stringCommand = new TestCommand<string>();

        var intHandlerMock = CommandHandlerMockBuilder
            .CreateMockCommandHandler<TestCommand<int>, int>(42);

        var stringHandlerMock =
            new Mock<ICommandHandler<TestCommand<string>, string>>();
        stringHandlerMock
            .Setup(h => h.HandleAsync(
                It.IsAny<TestCommand<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("result");

        var serviceProviderMock = new Mock<IServiceProvider>();
        var intHandlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var stringHandlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(typeof(TestCommand<string>), typeof(string));
        var intBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var stringBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<string>), typeof(string));
        var intBehaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(intBehaviourType);
        var stringBehaviourEnumerableType = typeof(IEnumerable<>).MakeGenericType(stringBehaviourType);

        serviceProviderMock
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns((Type serviceType) =>
            {
                if (serviceType == intHandlerType)
                    return intHandlerMock.Object;
                if (serviceType == stringHandlerType)
                    return stringHandlerMock.Object;
                if (serviceType == intBehaviourEnumerableType || serviceType == stringBehaviourEnumerableType)
                    return Enumerable.Empty<object>();
                return null;
            });

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var intResult = await dispatcher
            .HandleAsync(intCommand, TestContext.Current.CancellationToken);
        var stringResult = await dispatcher
            .HandleAsync(stringCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, intResult);
        Assert.Equal("result", stringResult);
    }
}