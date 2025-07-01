using FluentValidation;
using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.FinishSurvey.Validators;

internal sealed class FinishSurveyCommandValidator : AbstractValidator<FinishSurveyCommand>
{
    public FinishSurveyCommandValidator()
    {
        RuleFor(c => c.SurveyId)
            .NotEmpty();
    }
}