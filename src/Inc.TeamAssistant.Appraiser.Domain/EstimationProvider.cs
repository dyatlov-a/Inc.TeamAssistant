using Inc.TeamAssistant.Appraiser.Domain.Estimations;
using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Domain;

public static class EstimationProvider
{
	public static Estimation Create(StoryType storyType, int value)
	{
		return storyType switch
		{
			StoryType.Scrum => FibonacciEstimation.Create(value),
			StoryType.Kanban => TShirtEstimation.Create(value),
			_ => throw new TeamAssistantException("StoryType is not valid.")
		};
	}

	public static IReadOnlyCollection<Estimation> GetAssessments(StoryType storyType)
	{
		return storyType switch
		{
			StoryType.Scrum => FibonacciEstimation.Assessments,
			StoryType.Kanban => TShirtEstimation.Assessments,
			_ => throw new TeamAssistantException("StoryType is not valid.")
		};
	}

	public static string GetAcceptedValue(Story story)
	{
		ArgumentNullException.ThrowIfNull(story);

		if (!story.TotalValue.HasValue)
			return Estimation.UnknownValue;

		return story.StoryType switch
		{
			StoryType.Scrum => $"{story.TotalValue.Value}SP",
			StoryType.Kanban => story.TotalValue.Value.ToString().ToUpperInvariant(),
			_ => Estimation.UnknownValue
		};
	}

	public static string GetTotalValue(Story story)
	{
		ArgumentNullException.ThrowIfNull(story);

		if (!story.EstimateEnded)
			return Estimation.UnknownValue;

		return story.StoryType switch
		{
			StoryType.Scrum => CalculateMean(story)?.ToString(".## SP") ?? Estimation.UnknownValue,
			StoryType.Kanban => CalculateMedianValue(story)?.ToString().ToUpperInvariant() ?? Estimation.UnknownValue,
			_ => Estimation.UnknownValue
		};
	}
	
	public static string? CalculateMedian(Story story)
	{
		ArgumentNullException.ThrowIfNull(story);
		
		if (story.StoryType == StoryType.Kanban)
			return null;
		if (!story.EstimateEnded)
			return Estimation.UnknownValue;

		var values = story.StoryForEstimates
			.Where(i => i.Value > Estimation.NoIdea)
			.OrderBy(i => i.Value)
			.Select(i => i.Value)
			.ToArray();

		if (!values.Any())
			return null;

		var middle = values.Length / 2;

		var value = values.Length % 2 == 0
			? (values[middle] + values[middle - 1]) / 2m
			: values[middle];

		return value.ToString(".## SP");
	}

	private static decimal? CalculateMean(Story story)
	{
		ArgumentNullException.ThrowIfNull(story);
		
		var values = story.StoryForEstimates
			.Where(i => i.Value > Estimation.NoIdea)
			.Select(i => i.Value)
			.ToArray();

		var result = values.Any()
			? (decimal?)values.Sum() / values.Length
			: null;

		return result;
	}

	private static int? CalculateMedianValue(Story story)
	{
		ArgumentNullException.ThrowIfNull(story);
		
		var values = story.StoryForEstimates
			.Where(i => i.Value > Estimation.NoIdea)
			.OrderBy(i => i.Value)
			.Select(i => i.Value)
			.ToArray();

		if (!values.Any())
			return null;

		var middle = values.Length / 2;
		return values[middle];
	}
}