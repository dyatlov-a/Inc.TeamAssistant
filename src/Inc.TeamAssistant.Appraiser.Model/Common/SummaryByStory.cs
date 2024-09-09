using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Model.Common;

public sealed record SummaryByStory(
	Guid TeamId,
	LanguageId LanguageId,
	long ChatId,
	Guid StoryId,
	int? StoryExternalId,
	string StoryTitle,
	string StoryType,
	IReadOnlyCollection<string> StoryLinks,
	bool EstimateEnded,
    string Mean,
	string AcceptedValue,
    IReadOnlyCollection<EstimateItemDetails> Items,
	IReadOnlyCollection<EstimateDto> Assessments,
	bool Accepted,
	IReadOnlyCollection<EstimateDto> AssessmentsToAccept);