using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Commands.ReVoteEstimate;

public sealed record ReVoteEstimateResult(
    AssessmentSessionId AssessmentSessionId,
    LanguageId AssessmentSessionLanguageId,
    SummaryByStory Summary,
    bool EstimateEnded);