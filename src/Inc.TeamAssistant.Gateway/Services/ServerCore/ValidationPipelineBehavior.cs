using FluentValidation;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, token);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        return await next(token);
    }
}