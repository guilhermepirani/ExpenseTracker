using Mediator.Commands;
using Mediator.DependencyInjection;
using Mediator.Pipelines;
using Mediator.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Mediator.UnitTests;

/// <summary>
/// Tests for ConfigureServices.AddBehaviour dependency injection configuration.
/// </summary>
[Trait("Category", "AddBehaviour")]
public class AddBehaviourTests
{
    [Fact]
    public void WithValidBehaviour_RegistersWithServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var behaviourType = typeof(TestBehaviour<,>);

        // Act
        services.AddBehaviour(behaviourType);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var registeredBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviour = serviceProvider.GetService(registeredBehaviourType);
        Assert.NotNull(behaviour);
    }

    [Fact]
    public void WithValidBehaviour_RegistersAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var behaviourType = typeof(TestBehaviour<,>);

        // Act
        services.AddBehaviour(behaviourType);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Scoped lifetime means different instances for different scopes
        var registeredBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));

        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();
        var behaviour1 = scope1.ServiceProvider.GetService(registeredBehaviourType);
        var behaviour2 = scope2.ServiceProvider.GetService(registeredBehaviourType);

        Assert.NotNull(behaviour1);
        Assert.NotNull(behaviour2);
        Assert.NotSame(behaviour1, behaviour2);
    }

    [Fact]
    public void WithMultipleBehaviours_RegistersBoth()
    {
        // Arrange
        var services = new ServiceCollection();
        var behaviour1Type = typeof(TestBehaviour<,>);
        var behaviour2Type = typeof(CountingTestBehaviour<,>);

        // Act
        services.AddBehaviour(behaviour1Type);
        services.AddBehaviour(behaviour2Type);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - both behaviours are registered
        var registeredBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviours = serviceProvider.GetServices(registeredBehaviourType).ToList();
        Assert.Equal(2, behaviours.Count);
    }

    [Fact]
    public void WithBehaviourForDifferentRequest_RegistersForSpecificTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        var behaviourType = typeof(TestBehaviour<,>);

        // Act
        services.AddBehaviour(behaviourType);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - behaviour works for int result
        var intBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var intBehaviour = serviceProvider.GetService(intBehaviourType);
        Assert.NotNull(intBehaviour);

        // Assert - behaviour works for string result
        var stringBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<string>), typeof(string));
        var stringBehaviour = serviceProvider.GetService(stringBehaviourType);
        Assert.NotNull(stringBehaviour);
    }

    [Fact]
    public void WithNullBehaviour_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddBehaviour(null!));
    }

    [Fact]
    public void WithBehaviourForQueries_WorksWithQueryRequests()
    {
        // Arrange
        var services = new ServiceCollection();
        var behaviourType = typeof(TestBehaviour<,>);

        // Act
        services.AddBehaviour(behaviourType);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - behaviour works for query requests
        var queryBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestQuery<int>), typeof(int));
        var queryBehaviour = serviceProvider.GetService(queryBehaviourType);
        Assert.NotNull(queryBehaviour);
    }

    [Fact]
    public void WithMultipleBehavioursAddedInOrder_MaintainsRegistrationOrder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddBehaviour(typeof(TestBehaviour<,>));
        services.AddBehaviour(typeof(CountingTestBehaviour<,>));
        var serviceProvider = services.BuildServiceProvider();

        // Assert - behaviours are registered in order
        var registeredBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviours = serviceProvider.GetServices(registeredBehaviourType).ToList();

        Assert.Equal(2, behaviours.Count);
        Assert.IsType<TestBehaviour<TestCommand<int>, int>>(behaviours[0]);
        Assert.IsType<CountingTestBehaviour<TestCommand<int>, int>>(behaviours[1]);
    }

    [Fact]
    public void WithMultipleBehavioursOfDifferentTypes_BothResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddBehaviour(typeof(TestBehaviour<,>));
        services.AddBehaviour(typeof(CountingTestBehaviour<,>));

        var serviceProvider = services.BuildServiceProvider();

        // Assert - both behaviours registered
        var registeredBehaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(typeof(TestCommand<int>), typeof(int));
        var behaviours = serviceProvider.GetServices(registeredBehaviourType).ToList();
        Assert.Equal(2, behaviours.Count);
    }

    [Fact]
    public void WithInvalidBehaviourType_FailsAtBuildServiceProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        var invalidBehaviourType = typeof(string); // Not a valid behaviour type
        services.AddBehaviour(invalidBehaviourType);

        // Act & Assert - validation fails when building service provider
        Assert.Throws<ArgumentException>(() => services.BuildServiceProvider());
    }
}