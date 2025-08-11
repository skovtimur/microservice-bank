using FluentValidation;
using MediatR;

namespace AccountService.Validators;

public class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(f => f != null)
            .ToList();
        
#pragma warning disable CA1860 // ReSharper странно ругается на failures.Any(), предлогает написать проверка на != null, а после требует написать ее еще раз
        if (failures.Any())
#pragma warning restore CA1860
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}