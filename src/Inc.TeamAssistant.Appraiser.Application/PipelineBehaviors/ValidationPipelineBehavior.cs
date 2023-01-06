using FluentValidation;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.PipelineBehaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));
        if (next is null)
            throw new ArgumentNullException(nameof(next));

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        return await next();
    }
}