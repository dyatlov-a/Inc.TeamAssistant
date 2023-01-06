using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Common;

public record EstimateItemDetails(
	ParticipantId AppraiserId,
	string AppraiserName,
	int StoryExternalId,
	string HasValue,
	string DisplayValue);