using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class ActionItemConverter
{
    public static ActionItemDto ConvertTo(ActionItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        return new ActionItemDto(
            item.Id,
            item.RetroItemId,
            item.Created,
            item.Text,
            item.State.ToString());
    }
}