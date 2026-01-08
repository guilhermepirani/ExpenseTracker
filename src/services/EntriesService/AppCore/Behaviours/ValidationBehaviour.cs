using System.Net;
using System.Net.Cache;

using FluentValidation;

using Mediator;
using Mediator.Commands;
using Mediator.Pipelines;

using Serilog;
using Serilog.Context;

namespace AppCore.Behaviours;

public class ValidationBeheviour<TRequest, TResponse>
    : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBeheviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> HandleAsync(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (_validators.Any())
        {

            Log.Information("Validators are checking the request properties.");
            var validationContext = new ValidationContext<TRequest>(request);

            var validationResult = await Task.WhenAll(
                _validators.Select(validator =>
                    validator.ValidateAsync(
                        validationContext, cancellationToken)
            ));
            var validationFailures = validationResult.SelectMany(result =>
                result.Errors).Where(error => error is not null).ToList();

            if (validationFailures.Count > 0)
            {
                throw new ValidationException(validationFailures);
            }
        }

        Log.Information("Validators accepted the request properties.");

        var response = await next();

        return response;
    }
}