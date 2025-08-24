using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroColumnConverter
{
    public static RetroColumnDto ConvertTo(RetroColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);

        return new RetroColumnDto(
            column.Id,
            column.Name,
            column.Position,
            column.Color,
            column.Description);
    }
}