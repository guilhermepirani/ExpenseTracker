using AppCore.CreateEntry;

using Infra.CreateEntry;

using Microsoft.Extensions.DependencyInjection;

namespace Infra;

public static class RespositoryConfiguration
{
    public static IServiceCollection AddRespositories(
        this IServiceCollection services
    )
    {
        services.AddScoped<ICreateEntryRepository, CreateEntryRepository>();

        return services;
    }
}