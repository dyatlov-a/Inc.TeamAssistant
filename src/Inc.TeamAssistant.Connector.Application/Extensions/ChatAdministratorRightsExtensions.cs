using Telegram.Bot.Types;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class ChatAdministratorRightsExtensions
{
    public static IEnumerable<bool> Unwrap(this ChatAdministratorRights rights)
    {
        yield return rights.IsAnonymous;
        yield return rights.CanManageChat;
        yield return rights.CanDeleteMessages;
        yield return rights.CanManageVideoChats;
        yield return rights.CanRestrictMembers;
        yield return rights.CanPromoteMembers;
        yield return rights.CanChangeInfo;
        yield return rights.CanInviteUsers;
        yield return rights.CanPostMessages ?? false;
        yield return rights.CanEditMessages ?? false;
        yield return rights.CanPinMessages ?? false;
        yield return rights.CanManageTopics ?? false;
    }
}