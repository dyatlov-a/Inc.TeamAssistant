using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.LeaveFromRetro;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.LeaveFromRetro.Validators;

internal sealed class LeaveFromRetroCommandValidator : AbstractValidator<LeaveFromRetroCommand>
{
    public LeaveFromRetroCommandValidator()
    {
        RuleFor(c => c.ConnectionId)
            .NotEmpty();
        
        RuleFor(c => c.TeamId)
            .NotEmpty();
    }
}