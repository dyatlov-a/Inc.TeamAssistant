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
    private readonly IOnlinePersonStore _onlinePersonStore;

    public GetSurveyStateQueryHandler(
        ISurveyReader reader,
        IPersonResolver personResolver,
        IRoomPropertiesProvider propertiesProvider,
        IOnlinePersonStore onlinePersonStore)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
    }
    
    public async Task<GetSurveyStateResult> Handle(GetSurveyStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _personResolver.GetCurrentPerson();
        var onlinePersons = _onlinePersonStore.GetTickets(RoomId.CreateForSurvey(query.RoomId));
        var onlinePerson = onlinePersons.SingleOrDefault(p => p.Person.Id == currentPerson.Id);
        var roomProperties = await _propertiesProvider.Get(query.RoomId, token);
        var survey = await _reader.ReadLastSurvey(query.RoomId, SurveyStateRules.Active, token);
        
        var items = survey is not null
            ? await GetQuestions(survey.Id, currentPerson.Id, survey.TemplateId, token)
            : [];
        
        return new(
            survey?.Id,
            onlinePerson?.Finished ?? false,
            roomProperties.FacilitatorId,
            items,
            onlinePersons);
    }

    private async Task<IReadOnlyCollection<AnswerOnSurveyDto>> GetQuestions(
        Guid surveyId,
        long responderId,
        Guid templateId,
        CancellationToken token)
    {
        var surveyAnswer = await _reader.ReadAnswers([surveyId], token);
        var questions = await _reader.ReadQuestions(templateId, token);

        return questions
            .Select(q =>
            {
                var answer = surveyAnswer.SingleOrDefault(a => a.QuestionId == q.Id && a.ResponderId == responderId);

                return new AnswerOnSurveyDto(q.Id, q.Title, q.Text, answer?.Value, answer?.Comment);
            })
            .ToArray();
    }
}