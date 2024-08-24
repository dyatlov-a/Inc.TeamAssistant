using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.SetIntegrationProperties;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.SetIntegrationProperties.Validators;

internal sealed class SetIntegrationPropertiesCommandValidator : AbstractValidator<SetIntegrationPropertiesCommand>
{
    public SetIntegrationPropertiesCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();

        RuleFor(e => e.ProjectKey)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(e => e.ScrumMasterId)
            .NotEmpty();
    }
}