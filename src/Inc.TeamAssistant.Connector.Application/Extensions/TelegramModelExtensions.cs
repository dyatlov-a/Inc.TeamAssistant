using Inc.TeamAssistant.Connector.Domain;
using Telegram.Bot.Types;
using BotCommand = Telegram.Bot.Types.BotCommand;

namespace Inc.TeamAssistant.Connector.Application.Extensions;

internal static class TelegramModelExtensions
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
    
    public static bool FieldsEqual(this BotCommand first, BotCommand second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        
        return first.Command == second.Command && first.Description == second.Description;
    }
    
    public static bool TryConvert(this CommandScope commandScope, out BotCommandScope? botCommandScope)
    {
        botCommandScope = commandScope switch
        {
            CommandScope.AllGroupChats => BotCommandScope.AllGroupChats(),
            CommandScope.Default => BotCommandScope.Default(),
            _ => null
        };
        
        return botCommandScope is not null;
    }
}