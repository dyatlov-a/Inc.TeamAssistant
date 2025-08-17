using Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

namespace Inc.TeamAssistant.WebUI.Features.Survey;

public sealed class AnswerFromModel
{
    private readonly Guid _questionId;
    
    public IReadOnlyCollection<int> Values { get; private set; }
    public string Comment { get; private set; }

    public AnswerFromModel(AnswerOnSurveyDto item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _questionId = item.Id;
        Values = item.Value.HasValue ? new[] { item.Value.Value } : [];
        Comment = item.Comment ?? string.Empty;
    }

    public AnswerFromModel ChangeValues(IEnumerable<int> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        
        Values = values.ToArray();

        return this;
    }
    
    public AnswerFromModel ChangeComment(string value)
    {
        Comment = value;

        return this;
    }

    public SetAnswerCommand ToCommand(Guid surveyId, bool isEnd) => new(
        surveyId,
        _questionId,
        Values.Single(),
        string.IsNullOrWhiteSpace(Comment) ? null : Comment,
        isEnd);
}