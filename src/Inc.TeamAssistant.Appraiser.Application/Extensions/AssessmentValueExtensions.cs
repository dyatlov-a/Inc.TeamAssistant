using Inc.TeamAssistant.Appraiser.Domain;

namespace Inc.TeamAssistant.Appraiser.Application.Extensions;

public static class AssessmentValueExtensions
{
	public static string ToDisplayValue(this AssessmentValue.Value value)
	{
		return value switch
		{
			AssessmentValue.Value.None => "?",
			AssessmentValue.Value.NoIdea => "-",
			AssessmentValue.Value.More => "21+",
			_ => ((int)value).ToString()
		};
	}

	public static string ToDisplayValue(this decimal? value, bool estimateEnded) => estimateEnded && value.HasValue
        ? value.Value.ToString(".##")
        : "?";

	public static string ToDisplayHasValue(this AssessmentValue.Value value) => value == AssessmentValue.Value.None
		? "-"
		: "+";
}