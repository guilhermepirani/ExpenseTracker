using AppCore.Behaviours;
using AppCore.CreateEntry;
using AppCore.GetEntries;

using Mediator.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Configuration;

public static class PipelineConfiguration
{
    public static IServiceCollection AddPipelineBehaviours(
        this IServiceCollection services)
    {
        // Order of inclusion is order of execution
        services.AddBehaviour(typeof(LoggingBeheviour<,>));
        services.AddBehaviour(typeof(ExceptionHandlingBehaviour<,>));
        services.AddBehaviour(typeof(ValidationBeheviour<,>));

        return services;
    }
}