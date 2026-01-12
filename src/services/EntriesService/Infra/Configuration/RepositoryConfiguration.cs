using AppCore.CreateEntry;
using AppCore.DeleteEntry;
using AppCore.GetEntries;

using Infra.CreateEntry;
using Infra.DeleteEntry;

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
        services.AddScoped<IDeleteEntryRepository, DeleteEntryRespository>();

        return services;
    }
}