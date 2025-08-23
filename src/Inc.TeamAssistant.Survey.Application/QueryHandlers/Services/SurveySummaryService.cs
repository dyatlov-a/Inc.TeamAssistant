using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.Services;

internal sealed class SurveySummaryService
{
    private readonly ISurveyReader _reader;

    public SurveySummaryService(ISurveyReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<IReadOnlyCollection<SurveyQuestionDto>> GetSurveySummary(
        SurveyEntry surveyEntry,
        int top,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(surveyEntry);

        var templateId = surveyEntry.TemplateId;
        
        var surveys = await _reader.ReadSurveys(
            surveyEntry.Created,
            templateId,
            SurveyState.Completed,
            offset: 0,
            limit: top,
            token);
        var questions = await _reader.ReadQuestions(templateId, token);
        var surveyAnswers = await _reader.ReadAnswers(surveys.Select(s => s.Id).ToArray(), token);
        
        var surveysLookup = surveys.ToDictionary(s => s.Id, s => s.Created);
        var answersLookup = surveyAnswers.ToDictionary(
            k => (k.SurveyId, k.ResponderId, k.QuestionId),
            v => (v.Value, v.Comment));
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
                        responderId,
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

        return answers;
    }
}