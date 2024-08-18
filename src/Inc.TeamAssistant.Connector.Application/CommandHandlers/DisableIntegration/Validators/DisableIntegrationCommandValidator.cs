using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.DisableIntegration;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.DisableIntegration.Validators;

internal sealed class DisableIntegrationCommandValidator : AbstractValidator<DisableIntegrationCommand>
{
    public DisableIntegrationCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
    }
}