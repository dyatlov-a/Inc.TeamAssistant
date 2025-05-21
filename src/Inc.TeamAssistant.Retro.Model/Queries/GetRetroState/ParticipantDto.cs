using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record ParticipantDto(Person Person, int TotalVote);