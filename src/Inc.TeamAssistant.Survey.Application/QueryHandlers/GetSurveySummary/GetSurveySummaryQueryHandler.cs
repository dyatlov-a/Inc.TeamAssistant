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

        var surveys = await _reader.ReadSurveys(query.RoomId, SurveyState.Completed, query.Limit, token);
        if (!surveys.Any())
            return GetSurveySummaryResult.Empty;

        var surveyIds = surveys.Select(s => s.Id).ToArray();
        var lastSurvey = surveys.OrderByDescending(s => s.Created).First();
        var surveysLookup = surveys.ToDictionary(s => s.Id, s => s.Created);
        var questions = await _reader.ReadQuestions(lastSurvey.QuestionIds, token);
        var questionLookup = questions.ToDictionary(q => q.Id);
        var surveyAnswers = await _reader.ReadAnswers(surveyIds, token);

        var answers = new List<PersonAnswerDto>();
        var surveyQuestions = new Dictionary<Guid, List<QuestionAnswerDto>>();

        // TODO: change answer domain model
        foreach (var surveyAnswer in surveyAnswers.Where(sa => sa.SurveyId == lastSurvey.Id))
        foreach (var answer in surveyAnswer.Answers)
        {
            var questionTitleKey = questionLookup.TryGetValue(answer.QuestionId, out var question)
                ? question.Title
                : answer.QuestionId.ToString();

            answers.Add(new PersonAnswerDto(
                answer.QuestionId,
                questionTitleKey,
                surveyAnswer.OwnerId,
                answer.Value ?? 0,
                answer.Comment));
        }

        // TODO: change answer domain model
        foreach (var surveyAnswer in surveyAnswers)
        {
            var date = surveysLookup[surveyAnswer.SurveyId];

            foreach (var answer in surveyAnswer.Answers)
            {
                surveyQuestions.TryAdd(answer.QuestionId, new List<QuestionAnswerDto>());

                surveyQuestions[answer.QuestionId].Add(new QuestionAnswerDto(
                    date,
                    surveyAnswer.OwnerId,
                    answer.Value ?? 0));
            }
        }
        
        var result = new GetSurveySummaryResult(answers, surveyQuestions.Select(sq =>
        {
            var questionTitleKey = questionLookup.TryGetValue(sq.Key, out var question)
                ? question.Title
                : sq.Key.ToString();
            var questionTextKey = questionLookup.TryGetValue(sq.Key, out var text)
                ? text.Text
                : sq.Key.ToString();

            return new SurveyQuestionDto(sq.Key, questionTitleKey, questionTextKey, sq.Value);
        }).ToArray());

        return result;
    }
}