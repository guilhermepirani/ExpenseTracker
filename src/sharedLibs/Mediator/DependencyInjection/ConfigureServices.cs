using System.Reflection;

using Mediator.Commands;
using Mediator.Dispatcher;
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
}