using System.IO.Compression;
using System.Reflection;

using Mediator.Commands;
using Mediator.Dispatcher;
using Mediator.Pipelines;
using Mediator.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Mediator.DependencyInjection;

public static class ConfigureServices
{
    /// <summary>
    /// Adds the IDispatcher interface's transient implementation and auto resolves the Dependency Injection 
    /// of the Query and Command Handlers as scoped implementations via reflection discovery 
    /// at the given assembly and its dependencies.
    /// </summary>
    /// <param name="assembly">The base assembly to search for implementations.</param>
    public static void AddMediator(this IServiceCollection services,
        Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .FromAssemblyDependencies(assembly)
            .AddClasses(classes =>
                classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes =>
                classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.AddTransient<IDispatcher, CommandQueryDispatcher>();
    }

    /// <summary>
    /// Adds the given behaviour to the program. 
    /// If multiple, the order of addition is the order of execution.
    /// </summary>
    /// <param name="behaviour">The typeof of your behaviour</param>
    public static void AddBehaviour(
        this IServiceCollection services,
        Type behaviour)
    {
        services.AddScoped(typeof(IPipelineBehaviour<,>), behaviour);
    }
}