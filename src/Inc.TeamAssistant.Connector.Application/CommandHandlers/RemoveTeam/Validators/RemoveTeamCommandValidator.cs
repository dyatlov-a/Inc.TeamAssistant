using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeam;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeam.Validators;

internal sealed class RemoveTeamCommandValidator : AbstractValidator<RemoveTeamCommand>
{
    public RemoveTeamCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
    }
}