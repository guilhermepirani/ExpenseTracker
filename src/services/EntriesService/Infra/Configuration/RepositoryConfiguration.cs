using AppCore.CreateEntry;
using AppCore.GetEntries;

using Infra.CreateEntry;

using Microsoft.Extensions.DependencyInjection;

namespace Infra;

public static class RespositoryConfiguration
{
    public static IServiceCollection AddRespositories(
        this IServiceCollection services
    )
    {
        services.AddScoped<IGetEntriesRepository, GetEntriesRepository>();
        services.AddScoped<ICreateEntryRepository, CreateEntryRepository>();

        return services;
    }
}