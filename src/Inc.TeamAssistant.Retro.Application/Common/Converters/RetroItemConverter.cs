using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroItemConverter
{
    public static RetroItemDto ConvertTo(
        RetroItem item,
        RetroSessionState? state = null,
        IDictionary<Guid, int>? votesByPerson = null)
    {
        ArgumentNullException.ThrowIfNull(item);

        const int defaultVotes = 0;
        var votes = state switch
        {
            RetroSessionState.Prioritizing => votesByPerson?.TryGetValue(item.Id, out var value) == true ? value : null,
            RetroSessionState.Discussing or RetroSessionState.Finished => item.Votes,
            _ => null
        };
        
        return new RetroItemDto(
            item.Id,
            item.TeamId,
            item.Created,
            item.ColumnId,
            item.Position,
            item.Text,
            item.OwnerId,
            item.ParentId,
            votes ?? defaultVotes,
            item.Children.Select(c => new RetroItemDto(
                c.Id,
                c.TeamId,
                c.Created,
                c.ColumnId,
                c.Position,
                c.Text,
                c.OwnerId,
                c.ParentId,
                Votes: defaultVotes,
                Children: [])).ToArray());
    }
}