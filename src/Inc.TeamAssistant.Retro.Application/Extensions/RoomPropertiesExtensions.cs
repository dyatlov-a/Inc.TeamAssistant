using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Extensions;

internal static class RoomPropertiesExtensions
{
    public static RetroTypes RequiredRetroType(this RoomProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        return Enum.TryParse<RetroTypes>(properties.RetroType, ignoreCase: true, out var value)
            ? value
            : RetroTypes.Hidden;
    }
}