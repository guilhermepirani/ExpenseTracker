using System.Reflection;

using Asp.Versioning;
using Asp.Versioning.Builder;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EntriesService.Api;

public static class EndpointsConfiguration
{
    private static readonly ApiVersion V1 = new ApiVersion(1);

    public static IServiceCollection AddVersionedEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();
        services.TryAddEnumerable(serviceDescriptors);

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = V1;
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IApplicationBuilder MapVersionedEndpoints(
        this WebApplication app)
    {
        IEnumerable<IEndpoint> endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpoint>>();

        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(V1)
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder routeBuilder = app
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoints(routeBuilder);
        }

        return app;
    }
}