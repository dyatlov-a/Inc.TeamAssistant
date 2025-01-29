using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;
using Inc.TeamAssistant.Primitives.Extensions;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.CreateTeam.Validators;

internal sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(e => e.BotName)
            .NotEmpty();
        
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(255)
            .Must(e => !e.Contains(' '))
            .WithMessage("'PropertyName' must not contain spaces.")
            .Must(e => !e.HasCommand())
            .WithMessage("'PropertyName' must be a text value only.");
    }
}