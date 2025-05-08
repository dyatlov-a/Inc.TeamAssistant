using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.RemoveTeam;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.RemoveTeam.Validators;

internal sealed class RemoveTeamCommandValidator : AbstractValidator<RemoveTeamCommand>
{
    public RemoveTeamCommandValidator()
    {
        RuleFor(c => c.TeamId)
            .NotEmpty();
    }
}