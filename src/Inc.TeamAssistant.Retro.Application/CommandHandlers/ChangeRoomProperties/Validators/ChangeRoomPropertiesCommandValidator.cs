using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRoomProperties;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeRoomProperties.Validators;

internal sealed class ChangeRoomPropertiesCommandValidator : AbstractValidator<ChangeRoomPropertiesCommand>
{
    public ChangeRoomPropertiesCommandValidator()
    {
        RuleFor(c => c.RoomId)
            .NotEmpty();

        RuleFor(c => c.TemplateId)
            .NotEmpty()
            .When(c => c.TemplateId.HasValue);
        
        RuleFor(c => c.TimerDuration)
            .NotEmpty()
            .When(c => c.TimerDuration.HasValue);
        
        RuleFor(c => c.VoteCount)
            .NotEmpty()
            .When(c => c.VoteCount.HasValue);
    }
}