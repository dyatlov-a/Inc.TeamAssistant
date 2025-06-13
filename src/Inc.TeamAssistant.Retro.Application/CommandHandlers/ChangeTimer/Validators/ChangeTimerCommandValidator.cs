using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeTimer;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeTimer.Validators;

internal sealed class ChangeTimerCommandValidator : AbstractValidator<ChangeTimerCommand>
{
    public ChangeTimerCommandValidator()
    {
        RuleFor(c => c.RoomId)
            .NotEmpty();

        RuleFor(c => c.Duration)
            .GreaterThan(TimeSpan.Zero)
            .When(c => c.Duration.HasValue);
    }
}