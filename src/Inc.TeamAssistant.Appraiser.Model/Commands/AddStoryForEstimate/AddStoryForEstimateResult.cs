using Inc.TeamAssistant.Appraiser.Model.Common;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryForEstimate;

public sealed record AddStoryForEstimateResult(AssessmentSessionDetails AssessmentSessionDetails, bool IsUpdate);