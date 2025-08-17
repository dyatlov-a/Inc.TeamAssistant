using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveySummary;

internal sealed class GetSurveySummaryQueryHandler : IRequestHandler<GetSurveySummaryQuery, GetSurveySummaryResult>
{
    private readonly ISurveyReader _reader;
    private readonly IRoomPropertiesProvider _propertiesProvider;

    public GetSurveySummaryQueryHandler(ISurveyReader reader, IRoomPropertiesProvider propertiesProvider)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    }

    public async Task<GetSurveySummaryResult> Handle(GetSurveySummaryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var roomProperties = await _propertiesProvider.Get(query.RoomId, token);
        var surveys = await _reader.ReadSurveys(
            query.RoomId,
            roomProperties.SurveyTemplateId,
            SurveyState.Completed,
            offset: 0,
            query.Limit,
            token);
        
        if (!surveys.Any())
            return GetSurveySummaryResult.Empty;
        
        var questions = await _reader.ReadQuestions(roomProperties.SurveyTemplateId, token);
        var surveyAnswers = await _reader.ReadAnswers(surveys.Select(s => s.Id).ToArray(), token);
        
        var surveysLookup = surveys.ToDictionary(s => s.Id, s => s.Created);
        var answersLookup = surveyAnswers.ToDictionary(sa => (sa.SurveyId, sa.ResponderId, sa.QuestionId));
        var answers = new List<SurveyQuestionDto>();

        foreach (var survey in surveys)
        {
            var surveyDate = surveysLookup[survey.Id];
            var responderIds = answersLookup.Keys
                .Where(k => k.SurveyId == survey.Id)
                .Select(k => k.ResponderId)
                .Distinct()
                .ToArray();
            
            foreach (var question in questions)
            foreach (var responderId in responderIds)
                answers.Add(answersLookup.TryGetValue((survey.Id, responderId, question.Id), out var surveyAnswer)
                    ? new SurveyQuestionDto(
                        question.Id,
                        question.Title,
                        question.Text,
                        surveyDate,
                        surveyAnswer.ResponderId,
                        surveyAnswer.Value,
                        surveyAnswer.Comment)
                    : new SurveyQuestionDto(
                        question.Id,
                        question.Title,
                        question.Text,
                        surveyDate,
                        responderId,
                        Value: 0,
                        Comment: null));
        }

        return new GetSurveySummaryResult(answers);
    }
}