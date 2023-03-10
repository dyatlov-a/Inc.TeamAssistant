using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.DataAccess.InMemory;

internal static class Messages
{
    public static readonly MessageId ActiveSessionsByParticipantFound = new(nameof(ActiveSessionsByParticipantFound));
    public static readonly MessageId ActiveSessionsByIdFound = new(nameof(ActiveSessionsByIdFound));
}