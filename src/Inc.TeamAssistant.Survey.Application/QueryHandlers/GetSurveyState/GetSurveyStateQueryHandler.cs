using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveyState;

internal sealed class GetSurveyStateQueryHandler : IRequestHandler<GetSurveyStateQuery, GetSurveyStateResult>
{
    private readonly ISurveyReader _reader;
    private readonly IPersonResolver _personResolver;
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly ISurveyRepository _surveyRepository;
    private readonly IOnlinePersonStore _onlinePersonStore;

    public GetSurveyStateQueryHandler(
        ISurveyReader reader,
        IPersonResolver personResolver,
        IRoomPropertiesProvider propertiesProvider,
        ISurveyRepository surveyRepository,
        IOnlinePersonStore onlinePersonStore)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _surveyRepository = surveyRepository ?? throw new ArgumentNullException(nameof(surveyRepository));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
    }
    
    public async Task<GetSurveyStateResult> Handle(GetSurveyStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _personResolver.GetCurrentPerson();
        var onlinePersons = _onlinePersonStore.GetPersons(RoomId.CreateForSurvey(query.RoomId));
        var roomProperties = await _propertiesProvider.Get(query.RoomId, token);
        var survey = await _reader.Find(query.RoomId, SurveyStateRules.Active, token);
        
        var items = survey is not null
            ? await GetQuestions(survey.Id, currentPerson.Id, survey.QuestionIds, token)
            : [];
        var participants = onlinePersons
            .Select(op => new SurveyParticipantDto(op, Finished: false))
            .ToArray();

        return new(survey?.Id, roomProperties.FacilitatorId, items, participants);
    }

    private async Task<IReadOnlyCollection<SurveyQuestionDto>> GetQuestions(
        Guid surveyId,
        long ownerId,
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(questionIds);
        
        var answers = await _surveyRepository.Find(surveyId, ownerId, token);
        var questions = await _reader.ReadQuestions(questionIds, token);

        return questions
            .Select(q =>
            {
                var answer = answers?.Answers.SingleOrDefault(a => a.QuestionId == q.Id);

                return new SurveyQuestionDto(q.Id, q.Title, q.Text, answer?.Value, answer?.Comment);
            })
            .ToArray();
    }
}