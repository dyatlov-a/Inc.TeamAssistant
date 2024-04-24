using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record SummaryByStory(
	Guid TeamId,
	LanguageId LanguageId,
	long ChatId,
	Guid StoryId,
	int? StoryExternalId,
	string StoryTitle,
	IReadOnlyCollection<string> StoryLinks,
	bool EstimateEnded,
    string Total,
    IReadOnlyCollection<EstimateItemDetails> Items,
	IReadOnlyCollection<string> Assessments,
	bool Accepted);