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
	string AcceptedValue,
    IReadOnlyCollection<EstimateItemDetails> Items,
	IReadOnlyCollection<int> Assessments,
	bool Accepted,
	IReadOnlyCollection<string> AssessmentsToAccept);