using FluentValidation;
using Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Contracts;
using Inc.TeamAssistant.Connector.Domain;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.Begin.Validators;

internal sealed class BeginCommandValidator : AbstractValidator<BeginCommand>
{
    public BeginCommandValidator()
    {
        RuleFor(e => e.NextStage)
            .NotEqual(CommandStage.None);

        RuleFor(e => e.Command)
            .NotEmpty();
    }
}