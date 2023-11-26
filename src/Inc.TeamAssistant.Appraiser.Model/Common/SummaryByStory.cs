using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record SummaryByStory(
	Guid AssessmentSessionId,
	LanguageId AssessmentSessionLanguageId,
	long ChatId,
	int? StoryExternalId,
	string StoryTitle,
	IReadOnlyCollection<string> StoryLinks,
	bool EstimateEnded,
    string Total,
    IReadOnlyCollection<EstimateItemDetails> Items);