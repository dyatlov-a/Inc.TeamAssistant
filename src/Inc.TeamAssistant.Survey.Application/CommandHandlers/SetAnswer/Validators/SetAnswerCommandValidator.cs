using FluentValidation;
using Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.SetAnswer.Validators;

internal sealed class SetAnswerCommandValidator : AbstractValidator<SetAnswerCommand>
{
    public SetAnswerCommandValidator()
    {
        RuleFor(c => c.SurveyId)
            .NotEmpty();
        
        RuleFor(c => c.QuestionId)
            .NotEmpty();
        
        RuleFor(c => c.Value)
            .GreaterThan(0);
        
        RuleFor(c => c.Comment)
            .MaximumLength(2000);
    }
}