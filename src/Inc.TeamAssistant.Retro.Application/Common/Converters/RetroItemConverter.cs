using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroItemConverter
{
    public static RetroItemDto ConvertTo(RetroItem item, IDictionary<Guid, int>? votesByPerson = null)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new RetroItemDto(
            item.Id,
            item.TeamId,
            item.Created,
            item.ColumnId,
            item.Position,
            item.Text,
            item.OwnerId,
            item.ParentId,
            votesByPerson?.TryGetValue(item.Id, out var value) == true ? value : 0,
            item.Children.Select(c => new RetroItemDto(
                c.Id,
                c.TeamId,
                c.Created,
                c.ColumnId,
                c.Position,
                c.Text,
                c.OwnerId,
                c.ParentId,
                Votes: 0,
                Children: [])).ToArray());
    }
}