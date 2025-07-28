using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveyState;

internal sealed class GetSurveyStateQueryHandler : IRequestHandler<GetSurveyStateQuery, GetSurveyStateResult>
{
    private readonly ISurveyState _surveyState;
    private readonly ISurveyReader _reader;
    private readonly IPersonResolver _personResolver;
    private readonly IRoomPropertiesProvider _propertiesProvider;

    public GetSurveyStateQueryHandler(
        ISurveyState surveyState,
        ISurveyReader reader,
        IPersonResolver personResolver,
        IRoomPropertiesProvider propertiesProvider)
    {
        _surveyState = surveyState ?? throw new ArgumentNullException(nameof(surveyState));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    }
    
    public async Task<GetSurveyStateResult> Handle(GetSurveyStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _personResolver.GetCurrentPerson();
        var roomProperties = await _propertiesProvider.Get(query.RoomId, token);
        var survey = await _reader.Find(query.RoomId, SurveyStateRules.Active, token);
        var inProgress = survey is not null;
        
        var items = inProgress
            ? await GetQuestions(survey!.Id, currentPerson.Id, survey.QuestionIds, token)
            : [];

        return new(inProgress, roomProperties.FacilitatorId, items);
    }

    private async Task<IReadOnlyCollection<SurveyQuestionDto>> GetQuestions(
        Guid surveyId,
        long ownerId,
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(questionIds);
        
        var answers = _surveyState.GetAll(surveyId);
        var questions = await _reader.ReadQuestions(questionIds, token);

        return questions
            .Select(q =>
            {
                var answer = answers
                    .Where(a => a.OwnerId == ownerId)
                    .SelectMany(a => a.Answers)
                    .SingleOrDefault(a => a.QuestionId == q.Id);

                return new SurveyQuestionDto(q.Id, q.Title, q.Text, answer?.Value, answer?.Comment);
            })
            .ToArray();
    }
}