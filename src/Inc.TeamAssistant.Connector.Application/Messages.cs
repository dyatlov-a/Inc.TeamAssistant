using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Connector.Application;

internal static class Messages
{
    public static readonly MessageId Connector_SelectTeam = new(nameof(Connector_SelectTeam));
    public static readonly MessageId Connector_JoinToTeam = new(nameof(Connector_JoinToTeam));
    public static readonly MessageId Connector_JoinToTeamSuccess = new(nameof(Connector_JoinToTeamSuccess));
    public static readonly MessageId Connector_LeaveTeamSuccess = new(nameof(Connector_LeaveTeamSuccess));
    public static readonly MessageId Connector_BotNotFound = new(nameof(Connector_BotNotFound));
    public static readonly MessageId Connector_TeamForUserNotFound = new(nameof(Connector_TeamForUserNotFound));
    public static readonly MessageId Connector_ChangedPropertySuccess = new(nameof(Connector_ChangedPropertySuccess));
    public static readonly MessageId Connector_HelpText = new(nameof(Connector_HelpText));
    public static readonly MessageId Connector_BotShortDescription = new(nameof(Connector_BotShortDescription));
    public static readonly MessageId Connector_BotDescription = new(nameof(Connector_BotDescription));
    public static readonly MessageId Connector_HasNotRightsForRemoveTeam = new(nameof(Connector_HasNotRightsForRemoveTeam));
    public static readonly MessageId Connector_RemoveTeamSuccess = new(nameof(Connector_RemoveTeamSuccess));
    public static readonly MessageId Connector_Cancel = new(nameof(Connector_Cancel));
}