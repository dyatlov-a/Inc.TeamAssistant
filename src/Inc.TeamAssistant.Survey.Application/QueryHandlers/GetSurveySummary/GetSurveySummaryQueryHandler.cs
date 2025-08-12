using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveySummary;

internal sealed class GetSurveySummaryQueryHandler : IRequestHandler<GetSurveySummaryQuery, GetSurveySummaryResult>
{
    private readonly ISurveyReader _reader;
    private readonly ISurveyRepository _surveyRepository;

    public GetSurveySummaryQueryHandler(ISurveyReader reader, ISurveyRepository surveyRepository)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _surveyRepository = surveyRepository ?? throw new ArgumentNullException(nameof(surveyRepository));
    }

    public async Task<GetSurveySummaryResult> Handle(GetSurveySummaryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var surveyEntry = await query.SurveyId.Required(_surveyRepository.Find, token);
        var questions = await _reader.ReadQuestions(surveyEntry.QuestionIds, token);
        var surveyAnswers = await _reader.ReadAnswers(surveyEntry.RoomId, query.Limit, token);

        var items = questions
            .Select(q =>
            {
                var answerOnQuestion = new List<PersonAnswerDto>();
                var meanHistoryData = new List<(DateTimeOffset Created, int Value)>();

                foreach (var surveyAnswer in surveyAnswers)
                foreach (var answer in surveyAnswer.Answers.Where(a => a.QuestionId == q.Id))
                {
                    var value = answer.Value ?? 0;
                    
                    meanHistoryData.Add((surveyAnswer.Created, value));
                    
                    if (surveyAnswer.SurveyId == query.SurveyId)
                        answerOnQuestion.Add(new PersonAnswerDto(surveyAnswer.OwnerId, value, answer.Comment));
                }

                var mean = answerOnQuestion.Sum(a => a.Value) / answerOnQuestion.Count;
                var meanHistory = meanHistoryData
                    .OrderByDescending(i => i.Created)
                    .GroupBy(i => i.Created).Select(i => i.Sum(v => v.Value) / i.Count())
                    .ToArray();
                
                return new SurveyQuestionDto(q.Title, q.Text, mean, answerOnQuestion, meanHistory);
            })
            .ToArray();

        return new GetSurveySummaryResult(items);
    }
}