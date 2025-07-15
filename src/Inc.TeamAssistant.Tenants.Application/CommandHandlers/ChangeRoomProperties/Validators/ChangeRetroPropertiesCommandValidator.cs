using FluentValidation;
using Inc.TeamAssistant.Tenants.Model.Commands.ChangeRoomProperties;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.ChangeRoomProperties.Validators;

internal sealed class ChangeRetroPropertiesCommandValidator : AbstractValidator<ChangeRoomPropertiesCommand>
{
    public ChangeRetroPropertiesCommandValidator()
    {
        RuleFor(c => c.RoomId)
            .NotEmpty();

        RuleFor(c => c.RetroTemplateId)
            .NotEmpty()
            .When(c => c.RetroTemplateId.HasValue);
        
        RuleFor(c => c.TimerDuration)
            .NotEmpty()
            .When(c => c.TimerDuration.HasValue);
        
        RuleFor(c => c.VoteCount)
            .NotEmpty()
            .When(c => c.VoteCount.HasValue);
    }
}