using EntriesService.AppCore.Behaviours;

using Mediator.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace EntriesService.AppCore.Configuration;

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