using Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

namespace Inc.TeamAssistant.WebUI.Features.Survey;

public sealed class SurveyQuestionViewModel
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Text { get; private set; }
    public int? Value { get; private set; }
    public string? Comment { get; private set; }

    public SurveyQuestionViewModel(SurveyQuestionDto item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = item.Id;
        Title = item.Title;
        Text = item.Text;
        Value = item.Value;
        Comment = item.Comment;
    }

    public SurveyQuestionViewModel ChangeValue(int? value)
    {
        Value = value;

        return this;
    }
    
    public SurveyQuestionViewModel ChangeComment(string? value)
    {
        Comment = value;

        return this;
    }

    public SetAnswerCommand ToCommand(Guid surveyId) => new(surveyId, Id, Value, Comment);
}