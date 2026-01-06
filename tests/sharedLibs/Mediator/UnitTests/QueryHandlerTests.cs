using Mediator.Dispatcher;
using Mediator.Queries;

using Moq;

namespace Mediator.UnitTests;

/// <summary>
/// Tests for query dispatch functionality.
/// </summary>
public class QueryHandlerTests
{
    [Fact]
    public async Task WithValidQuery_ReturnsCorrectResult()
    {
        // Arrange
        const int expectedResult = 100;
        var query = new TestQuery<int>();
        var handlerMock = QueryHandlerMockBuilder
            .CreateMockQueryHandler<TestQuery<int>, int>(expectedResult);
        var serviceProviderMock = QueryHandlerMockBuilder
            .CreateMockServiceProviderWithQueryHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.HandleAsync(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedResult, result);
        handlerMock.Verify(h => h.HandleAsync(
            query,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task WithStringResult_ReturnsCorrectResult()
    {
        // Arrange
        const string expectedResult = "query result";
        var query = new TestQuery<string>();
        var handlerMock = QueryHandlerMockBuilder
            .CreateMockQueryHandler<TestQuery<string>, string>(expectedResult);
        var serviceProviderMock = QueryHandlerMockBuilder
            .CreateMockServiceProviderWithQueryHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.HandleAsync(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expectedResult, result);
        handlerMock.Verify(h => h.HandleAsync(
            query,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WithComplexType_ReturnsCorrectResult()
    {
        // Arrange
        var expectedResult = new { Id = 1, Name = "Query Result" };
        var query = new TestQuery<object>();
        var handlerMock = new Mock<IQueryHandler<TestQuery<object>, object>>();
        handlerMock
            .Setup(h => h.HandleAsync(
                It.IsAny<TestQuery<object>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(typeof(TestQuery<object>), typeof(object));
        serviceProviderMock
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handlerMock.Object);

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        var result = await dispatcher.HandleAsync(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task HandlerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var query = new TestQuery<int>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));

        serviceProviderMock
            .Setup(sp => sp.GetService(handlerType))
            .Returns((object?)null);

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.HandleAsync(
                query,
                TestContext.Current.CancellationToken));
        Assert.Contains("No service", exception.Message);
    }

    [Fact]
    public async Task HandlerThrowsException_ExceptionPropagated()
    {
        // Arrange
        var query = new TestQuery<int>();
        var expectedException = new InvalidOperationException("Query handler error");
        var handlerMock = new Mock<IQueryHandler<TestQuery<int>, int>>();
        handlerMock
            .Setup(h => h.HandleAsync(
                It.IsAny<TestQuery<int>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));
        serviceProviderMock
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handlerMock.Object);

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.HandleAsync(
                query,
                TestContext.Current.CancellationToken));
        Assert.Equal("Query handler error", exception.Message);
    }

    [Fact]
    public async Task CancellationTokenPropagation_TokenPassedToHandler()
    {
        // Arrange
        var query = new TestQuery<int>();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var handlerMock = QueryHandlerMockBuilder
            .CreateMockQueryHandler<TestQuery<int>, int>(100);
        var serviceProviderMock = QueryHandlerMockBuilder
            .CreateMockServiceProviderWithQueryHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        await dispatcher.HandleAsync(query, cancellationToken);

        // Assert
        handlerMock.Verify(
            h => h.HandleAsync(query, cancellationToken),
            Times.Once,
            "Cancellation token should be passed to the handler");
    }

    [Fact]
    public async Task WithPreCancelledToken_ThrowsTaskCanceledException()
    {
        // Arrange
        var query = new TestQuery<int>();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        var cancellationToken = cancellationTokenSource.Token;

        var handlerMock = new Mock<IQueryHandler<TestQuery<int>, int>>();
        handlerMock
            .Setup(h => h.HandleAsync(
                It.IsAny<TestQuery<int>>(),
                It.IsAny<CancellationToken>()))
            .Returns((TestQuery<int> _, CancellationToken ct) =>
            {
                if (ct.IsCancellationRequested)
                    return Task.FromCanceled<int>(ct);
                return Task.FromResult(100);
            });

        var serviceProviderMock = new Mock<IServiceProvider>();
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));
        serviceProviderMock
            .Setup(sp => sp.GetService(handlerType))
            .Returns(handlerMock.Object);

        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TaskCanceledException>(
            () => dispatcher.HandleAsync(query, cancellationToken));
        Assert.IsType<TaskCanceledException>(exception);
    }

    [Fact]
    public async Task WithDefaultCancellationToken_UsesDefault()
    {
        // Arrange
        var query = new TestQuery<int>();
        var handlerMock = QueryHandlerMockBuilder
            .CreateMockQueryHandler<TestQuery<int>, int>(100);
        var serviceProviderMock = QueryHandlerMockBuilder
            .CreateMockServiceProviderWithQueryHandler(handlerMock);
        var dispatcher = new CommandQueryDispatcher(serviceProviderMock.Object);

        // Act
        await dispatcher.HandleAsync(query, TestContext.Current.CancellationToken);

        // Assert
        handlerMock.Verify(
            h => h.HandleAsync(query, TestContext.Current.CancellationToken),
            Times.Once);
    }
}