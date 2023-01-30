using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.ShowParticipants;

public sealed record ShowParticipantsResult(
    LanguageId AssessmentSessionLanguageId,
    IReadOnlyCollection<string> Appraisers);