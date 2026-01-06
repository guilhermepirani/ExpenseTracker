using System.Reflection;

using Mediator.Commands;
using Mediator.DependencyInjection;
using Mediator.Dispatcher;
using Mediator.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Mediator.UnitTests;

/// <summary>
/// Tests for ConfigureServices dependency injection configuration.
/// </summary>
public class ConfigureServicesTests
{
    [Fact]
    public void WithValidAssembly_RegistersCommandHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var handler = serviceProvider.GetService(handlerType);
        Assert.NotNull(handler);
    }

    [Fact]
    public void WithValidAssembly_RegistersQueryHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntQueryHandler).Assembly;

        // Act
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));
        var handler = serviceProvider.GetService(handlerType);
        Assert.NotNull(handler);
    }

    [Fact]
    public void WithValidAssembly_RegistersDispatcher()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dispatcher = serviceProvider.GetService<IDispatcher>();
        Assert.NotNull(dispatcher);
        Assert.IsType<CommandQueryDispatcher>(dispatcher);
    }

    [Fact]
    public void HandlerRegisteredAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        services.AddMediator(assembly);

        // Assert
        var commandHandlerDescriptor = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(ICommandHandler<TestCommand<int>, int>));
        Assert.NotNull(commandHandlerDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, commandHandlerDescriptor.Lifetime);

        var queryHandlerDescriptor = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IQueryHandler<TestQuery<int>, int>));
        Assert.NotNull(queryHandlerDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, queryHandlerDescriptor.Lifetime);
    }

    [Fact]
    public void DispatcherRegisteredAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        services.AddMediator(assembly);

        // Assert
        var dispatcherDescriptor = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IDispatcher));
        Assert.NotNull(dispatcherDescriptor);
        Assert.Equal(ServiceLifetime.Transient, dispatcherDescriptor.Lifetime);
    }

    [Fact]
    public void RegistersHandlersAsImplementedInterfaces()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Verify handlers are registered by interface, not concrete type
        var commandHandlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var handler = serviceProvider.GetService(commandHandlerType);
        Assert.NotNull(handler);
        Assert.True(
            commandHandlerType.IsInstanceOfType(handler),
            "Handler should be registered as its interface");
    }

    [Fact]
    public async Task IntegrationTest_DispatcherCanExecuteCommand()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var command = new TestCommand<string> { Payload = "test payload" };

        // Act
        var result = await dispatcher.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task IntegrationTest_DispatcherCanExecuteQuery()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntQueryHandler).Assembly;
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var query = new TestQuery<string> { Payload = "test payload" };

        // Act
        var result = await dispatcher.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public void ResolvedHandlersAreDistinct()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Act - Create separate scopes to get distinct instances
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();
        var handler1 = scope1.ServiceProvider
            .GetService(typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int)));
        var handler2 = scope2.ServiceProvider
            .GetService(typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int)));

        // Assert - Scoped lifetime means new instance per scope
        Assert.NotNull(handler1);
        Assert.NotNull(handler2);
        Assert.NotSame(handler1, handler2);
    }

    [Fact]
    public void DispatcherInstancesAreDistinct()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var dispatcher1 = serviceProvider.GetService<IDispatcher>();
        var dispatcher2 = serviceProvider.GetService<IDispatcher>();

        // Assert - Transient lifetime means new instance every time
        Assert.NotNull(dispatcher1);
        Assert.NotNull(dispatcher2);
        Assert.NotSame(dispatcher1, dispatcher2);
    }

    [Fact]
    public void WithNullAssembly_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            services.AddMediator(null!));
    }

    [Fact]
    public void WithEmptyAssemblyWithoutHandlers_StillRegistersDispatcher()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(object).Assembly; // System assembly without custom handlers

        // Act
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dispatcher = serviceProvider.GetService<IDispatcher>();
        Assert.NotNull(dispatcher);
        Assert.IsType<CommandQueryDispatcher>(dispatcher);
    }

    [Fact]
    public void MultipleCallsWithSameAssembly_AddsHandlersMultipleTimes()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        services.AddMediator(assembly);
        services.AddMediator(assembly);

        // Assert - Verify handlers appear in the service collection multiple times
        var commandHandlerDescriptors = services
            .Where(sd => sd.ServiceType == typeof(
                ICommandHandler<TestCommand<int>, int>))
            .ToList();
        Assert.True(
            commandHandlerDescriptors.Count >= 2,
            "Multiple calls to AddMediator should register handlers multiple times");
    }

    [Fact]
    public void CommandHandlersDiscoveredFromDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        // Use the test assembly which contains test handler implementations
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Verify that handlers from the assembly and its dependencies are registered
        var handlers = services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
            .ToList();
        Assert.NotEmpty(handlers);
    }

    [Fact]
    public void QueryHandlersDiscoveredFromDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntQueryHandler).Assembly;

        // Act
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Verify that query handlers from the assembly and its dependencies are registered
        var handlers = services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
            .ToList();
        Assert.NotEmpty(handlers);
    }

    [Fact]
    public async Task DifferentCommandTypes_AreHandledByDistinctHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var intCommand = new TestCommand<int>();
        var stringCommand = new TestCommand<string>();

        // Act
        var intResult = await dispatcher
            .HandleAsync(intCommand, CancellationToken.None);
        var stringResult = await dispatcher
            .HandleAsync(stringCommand, CancellationToken.None);

        // Assert
        Assert.IsType<int>(intResult);
        Assert.IsType<string>(stringResult);
    }

    [Fact]
    public async Task DifferentQueryTypes_AreHandledByDistinctHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntQueryHandler).Assembly;
        services.AddMediator(assembly);
        var serviceProvider = services.BuildServiceProvider();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var intQuery = new TestQuery<int>();
        var stringQuery = new TestQuery<string>();

        // Act
        var intResult = await dispatcher
            .HandleAsync(intQuery, CancellationToken.None);
        var stringResult = await dispatcher
            .HandleAsync(stringQuery, CancellationToken.None);

        // Assert
        Assert.IsType<int>(intResult);
        Assert.IsType<string>(stringResult);
    }

    [Fact]
    public void FromAssemblies_SpecificAssemblyHandlersAreDiscovered()
    {
        // Arrange
        var services = new ServiceCollection();
        // Use the UnitTests assembly which contains IntCommandHandler, StringCommandHandler, etc.
        var testAssembly = typeof(IntCommandHandler).Assembly;

        // Act
        // This calls .FromAssemblies(assembly) which scans the specified assembly
        services.AddMediator(testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Verify handlers from the specified assembly are discovered and registered
        var intCommandHandlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var stringCommandHandlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<string>), typeof(string));
        var intQueryHandlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));
        var stringQueryHandlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(typeof(TestQuery<string>), typeof(string));

        var intCommandHandler = serviceProvider.GetService(intCommandHandlerType);
        var stringCommandHandler = serviceProvider.GetService(stringCommandHandlerType);
        var intQueryHandler = serviceProvider.GetService(intQueryHandlerType);
        var stringQueryHandler = serviceProvider.GetService(stringQueryHandlerType);

        Assert.NotNull(intCommandHandler);
        Assert.NotNull(stringCommandHandler);
        Assert.NotNull(intQueryHandler);
        Assert.NotNull(stringQueryHandler);

        // Verify they are from the correct assembly
        Assert.Equal(testAssembly, intCommandHandler.GetType().Assembly);
        Assert.Equal(testAssembly, stringCommandHandler.GetType().Assembly);
        Assert.Equal(testAssembly, intQueryHandler.GetType().Assembly);
        Assert.Equal(testAssembly, stringQueryHandler.GetType().Assembly);
    }

    [Fact]
    public void FromAssemblies_MultipleHandlersInSameAssembly()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(IntCommandHandler).Assembly;

        // Act
        // Verify that .FromAssemblies(assembly) discovers ALL handler implementations in that assembly
        services.AddMediator(assembly);

        // Assert
        var allCommandHandlers = services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
            .ToList();
        var allQueryHandlers = services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
            .ToList();

        // Should have discovered both IntCommandHandler and StringCommandHandler
        Assert.True(allCommandHandlers.Count >= 2,
            $"Expected at least 2 command handlers, but found {allCommandHandlers.Count}");
        // Should have discovered both IntQueryHandler and StringQueryHandler
        Assert.True(allQueryHandlers.Count >= 2,
            $"Expected at least 2 query handlers, but found {allQueryHandlers.Count}");

        // Verify all are from the assembly specified in .FromAssemblies()
        foreach (var descriptor in allCommandHandlers)
        {
            Assert.Equal(assembly, descriptor.ImplementationType?.Assembly);
        }
        foreach (var descriptor in allQueryHandlers)
        {
            Assert.Equal(assembly, descriptor.ImplementationType?.Assembly);
        }
    }

    [Fact]
    public void FromAssemblies_WithDifferentAssembly_DiscoveryIsolated()
    {
        // Arrange
        var services = new ServiceCollection();
        // Using System assembly (which has no handlers) to verify
        // FromAssemblies respects the assembly parameter
        var systemAssembly = typeof(object).Assembly;

        // Act
        services.AddMediator(systemAssembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - System assembly shouldn't have our handlers
        var intCommandHandlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var handler = serviceProvider.GetService(intCommandHandlerType);

        // Should not find handlers from System assembly
        Assert.Null(handler);
    }

    [Fact]
    public void FromAssembliesAndDependencies_BothAreScanned()
    {
        // Arrange
        var services = new ServiceCollection();
        var testAssembly = typeof(IntCommandHandler).Assembly;

        // Act
        // This calls .FromAssemblies(assembly) AND .FromAssemblyDependencies(assembly)
        services.AddMediator(testAssembly);

        // Assert - Verify that handlers are found AND dispatcher is available from dependencies
        var commandHandlers = services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
            .ToList();
        var queryHandlers = services.Where(sd =>
            sd.ServiceType.IsGenericType &&
            sd.ServiceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
            .ToList();
        var dispatcherRegistration = services.FirstOrDefault(sd =>
            sd.ServiceType == typeof(IDispatcher));

        // Handlers should be found from the main assembly
        Assert.NotEmpty(commandHandlers);
        Assert.NotEmpty(queryHandlers);
        // Dispatcher should be registered (it's in the dependencies of testAssembly)
        Assert.NotNull(dispatcherRegistration);
    }
}