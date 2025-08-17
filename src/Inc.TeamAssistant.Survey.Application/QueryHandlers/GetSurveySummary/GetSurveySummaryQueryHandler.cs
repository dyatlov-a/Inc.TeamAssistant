using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveySummary;

internal sealed class GetSurveySummaryQueryHandler : IRequestHandler<GetSurveySummaryQuery, GetSurveySummaryResult>
{
    private readonly ISurveyReader _reader;

    public GetSurveySummaryQueryHandler(ISurveyReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetSurveySummaryResult> Handle(GetSurveySummaryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var lastSurvey = await _reader.Find(query.RoomId, [SurveyState.Completed], token);
        if (lastSurvey is null)
            return GetSurveySummaryResult.Empty;

        var history = await _reader.ReadSurveys(
            lastSurvey.RoomId,
            lastSurvey.TemplateId,
            SurveyState.Completed,
            offset: 1,
            query.Limit,
            token);
        var surveys = history.Append(lastSurvey).ToArray();
        var surveyIds = surveys.Select(s => s.Id).ToArray();
        var surveysLookup = surveys.ToDictionary(s => s.Id, s => s.Created);
        var questions = await _reader.ReadQuestions(lastSurvey.QuestionIds, token);
        var questionLookup = questions.ToDictionary(q => q.Id);
        var surveyAnswers = await _reader.ReadAnswers(surveyIds, token);
        var answersLookup = surveyAnswers
            .Where(sa => sa.SurveyId == lastSurvey.Id)
            .ToDictionary(sa => (sa.QuestionId, sa.ResponderId));
        var responders = answersLookup.Keys.Select(a => a.ResponderId).Distinct().ToArray();

        var answers = lastSurvey.QuestionIds
            .SelectMany(q => responders.Select(r =>
            {
                var questionTitleKey = questionLookup.TryGetValue(q, out var question) ? question.Title : q.ToString();

                return answersLookup.TryGetValue((q, r), out var value)
                    ? new PersonAnswerDto(
                        value.QuestionId,
                        questionTitleKey,
                        value.ResponderId,
                        value.Value,
                        value.Comment)
                    : new PersonAnswerDto(q, questionTitleKey, r, Value: 0, Comment: null);
            }))
            .ToArray();
        var surveyQuestions = surveyAnswers
            .GroupBy(sa => sa.QuestionId)
            .Select(sa =>
            {
                var question = questionLookup.GetValueOrDefault(sa.Key);

                return new SurveyQuestionDto(
                    sa.Key,
                    question?.Title ?? sa.Key.ToString(),
                    question?.Text ?? sa.Key.ToString(),
                    sa.Select(i => new QuestionAnswerDto(surveysLookup[i.SurveyId], i.ResponderId, i.Value)).ToArray());
            })
            .ToArray();

        return new GetSurveySummaryResult(answers, surveyQuestions, responders.Length);
    }
}