using FluentValidation;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetRetroAssessment.Validators;

internal sealed class SetRetroAssessmentCommandValidator : AbstractValidator<SetRetroAssessmentCommand>
{
    public SetRetroAssessmentCommandValidator()
    {
        RuleFor(c => c.SessionId)
            .NotEmpty();
        
        RuleFor(c => c.Value)
            .InclusiveBetween(1, 5)
            .WithMessage("Value must be between 1 and 5.");
    }
}