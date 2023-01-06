namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record SummaryByStory(
	long ChatId,
	StoryDetails Story,
    string Total,
    IReadOnlyCollection<EstimateItemDetails> Items);