using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.CreateTeam;

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
            .Must(e => !e.StartsWith("/"))
            .WithMessage("Please enter text value.");
    }
}