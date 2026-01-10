using FluentValidation;

using Mediator;
using Mediator.Pipelines;

using Serilog;

namespace AppCore.Behaviours;

public class ValidationBeheviour<TRequest, TResponse>
    : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
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
                Log.Information("Validators refused the request properties: {failures}.",
                    validationFailures);

                return ResultFactory.CreateFailure<TResponse>(
                    validationFailures.Select(f => f.ErrorMessage).ToArray());
            }
        }

        Log.Information("Validators accepted the request properties.");

        return await next();
    }
}