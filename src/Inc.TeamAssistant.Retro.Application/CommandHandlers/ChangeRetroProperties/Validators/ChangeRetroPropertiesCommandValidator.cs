using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeRetroProperties.Validators;

internal sealed class ChangeRetroPropertiesCommandValidator : AbstractValidator<ChangeRetroPropertiesCommand>
{
    public ChangeRetroPropertiesCommandValidator()
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