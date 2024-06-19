using Inc.TeamAssistant.Appraiser.Domain;

namespace Inc.TeamAssistant.Appraiser.Application.Extensions;

public static class AssessmentValueExtensions
{
	public static string ToDisplayValue(this AssessmentValue.Value value, StoryType storyType)
	{
		return value switch
		{
			AssessmentValue.Value.None => "?",
			AssessmentValue.Value.NoIdea => "?",
			AssessmentValue.Value.More when storyType == StoryType.Scrum => "21+",
			AssessmentValue.Value.More => "XXL+",
			_ => storyType == StoryType.Scrum ? ((int)value).ToString() : value.ToString().ToUpperInvariant()
		};
	}

	public static string ToDisplayHasValue(this AssessmentValue.Value value) => value == AssessmentValue.Value.None
		? "-"
		: "+";
}