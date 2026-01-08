using AppCore.CreateEntry;

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

        return services;
    }
}