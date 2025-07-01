using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Model.Queries.GetPersonSurvey;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetPersonSurvey;

internal sealed class GetPersonSurveyQueryHandler : IRequestHandler<GetPersonSurveyQuery, GetPersonSurveyResult>
{
    private readonly ISurveyState _surveyState;
    private readonly ISurveyReader _reader;
    private readonly ISurveyRepository _repository;
    private readonly IPersonResolver _personResolver;

    public GetPersonSurveyQueryHandler(
        ISurveyState surveyState,
        ISurveyReader reader,
        ISurveyRepository repository,
        IPersonResolver personResolver)
    {
        _surveyState = surveyState ?? throw new ArgumentNullException(nameof(surveyState));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }
    
    public async Task<GetPersonSurveyResult> Handle(GetPersonSurveyQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _personResolver.GetCurrentPerson();
        var ownerId = currentPerson.Id;
        var answers = _surveyState.GetAll(query.SurveyId);
        var survey = await query.SurveyId.Required(_repository.Find, token);
        var questions = await _reader.ReadQuestions(survey.QuestionIds, token);

        var items = questions
            .Select(q =>
            {
                var answer = answers
                    .Where(a => a.OwnerId == ownerId)
                    .SelectMany(a => a.Answers)
                    .SingleOrDefault(a => a.QuestionId == q.Id);

                return new SurveyQuestionDto(q.Id, q.Title, q.Text, answer?.Value, answer?.Comment);
            })
            .ToArray();

        return new(items);
    }
}