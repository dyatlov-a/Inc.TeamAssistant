using FluentValidation;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Model.Commands.FinishAnswer;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.FinishAnswer.Validators;

internal sealed class FinishAnswerCommandValidator : AbstractValidator<FinishAnswerCommand>
{
    private readonly ISurveyState _surveyState;
    private readonly IPersonResolver _personResolver;
    
    public FinishAnswerCommandValidator(ISurveyState surveyState, IPersonResolver personResolver)
    {
        _surveyState = surveyState ?? throw new ArgumentNullException(nameof(surveyState));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        
        RuleFor(c => c.SurveyId)
            .NotEmpty()
            .Must(AnswerExists)
            .WithMessage("Person answer is not exists");
    }

    private bool AnswerExists(Guid surveyId)
    {
        var currentPerson = _personResolver.GetCurrentPerson();
        var answer = _surveyState.Get(surveyId, currentPerson.Id);

        return answer is not null;
    }
}