using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.JoinToTeam;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.JoinToTeam.Validators;

internal sealed class JoinToTeamCommandValidator : AbstractValidator<JoinToTeamCommand>
{
    public JoinToTeamCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
    }
}