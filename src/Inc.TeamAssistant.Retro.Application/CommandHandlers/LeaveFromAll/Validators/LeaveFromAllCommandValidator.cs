using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.LeaveFromAll;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.LeaveFromAll.Validators;

internal sealed class LeaveFromAllCommandValidator : AbstractValidator<LeaveFromAllCommand>
{
    public LeaveFromAllCommandValidator()
    {
        RuleFor(c => c.ConnectionId)
            .NotEmpty();
    }
}