using AppCore.Features.CreateEntry.V1;
using AppCore.Features.DeleteEntry.V1;
using AppCore.Features.GetEntries.V1;
using Infra.Features.CreateEntry.V1;
using Infra.Features.DeleteEntry.V1;
using Infra.Features.GetEntries.V1;

using Microsoft.Extensions.DependencyInjection;

namespace Infra.Configuration;

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