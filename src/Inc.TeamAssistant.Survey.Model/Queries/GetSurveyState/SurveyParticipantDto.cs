using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

public sealed record SurveyParticipantDto(Person Person, bool Finished);