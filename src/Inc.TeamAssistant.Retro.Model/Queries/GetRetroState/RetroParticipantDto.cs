using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;

public sealed record RetroParticipantDto(Person Person, int TotalVote, bool Finished, bool HandRaised);