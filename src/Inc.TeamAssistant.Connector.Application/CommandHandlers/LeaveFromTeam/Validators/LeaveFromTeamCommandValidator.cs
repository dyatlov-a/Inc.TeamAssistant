using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Commands.LeaveFromTeam;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.LeaveFromTeam.Validators;

internal sealed class LeaveFromTeamCommandValidator : AbstractValidator<LeaveFromTeamCommand>
{
    public LeaveFromTeamCommandValidator()
    {
        RuleFor(e => e.TeamId)
            .NotEmpty();
    }
}