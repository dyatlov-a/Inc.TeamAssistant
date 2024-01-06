using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Application;

internal static class Messages
{
    public static MessageId Connector_EnterTeamName = new(nameof(Connector_EnterTeamName));
    public static MessageId Connector_SelectTeam = new(nameof(Connector_SelectTeam));
    public static MessageId Connector_JoinToTeam = new(nameof(Connector_JoinToTeam));
    public static MessageId Connector_JoinToTeamSuccess = new(nameof(Connector_JoinToTeamSuccess));
    public static MessageId Connector_EnterTextError = new(nameof(Connector_EnterTextError));
    public static MessageId Connector_TeamNotFound = new(nameof(Connector_TeamNotFound));
    public static MessageId Connector_LeaveTeamSuccess = new(nameof(Connector_LeaveTeamSuccess));
}