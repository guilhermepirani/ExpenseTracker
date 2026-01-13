using AppCore.Features.CreateEntry.V1;
using AppCore.Features.GetEntries.V1;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Configuration;

public static class ValidatorConfiguration
{
    public static IServiceCollection AddValidators(
        this IServiceCollection services
    )
    {
        services.AddScoped
            <IValidator<CreateEntryCommand>, CreatyEntryValidator>();
        services.AddScoped
            <IValidator<GetEntriesQuery>, GetEntriesQueryValidator>();

        return services;
    }
}