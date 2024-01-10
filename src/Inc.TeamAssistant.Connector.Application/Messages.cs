using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Application;

internal static class Messages
{
    public static readonly MessageId Connector_EnterTeamName = new(nameof(Connector_EnterTeamName));
    public static readonly MessageId Connector_SelectTeam = new(nameof(Connector_SelectTeam));
    public static readonly MessageId Connector_JoinToTeam = new(nameof(Connector_JoinToTeam));
    public static readonly MessageId Connector_JoinToTeamSuccess = new(nameof(Connector_JoinToTeamSuccess));
    public static readonly MessageId Connector_EnterTextError = new(nameof(Connector_EnterTextError));
    public static readonly MessageId Connector_TeamNotFound = new(nameof(Connector_TeamNotFound));
    public static readonly MessageId Connector_LeaveTeamSuccess = new(nameof(Connector_LeaveTeamSuccess));
}