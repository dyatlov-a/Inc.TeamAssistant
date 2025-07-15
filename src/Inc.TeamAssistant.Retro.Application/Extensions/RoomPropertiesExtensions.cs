using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Retro.Domain;

namespace Inc.TeamAssistant.Retro.Application.Extensions;

public static class RoomPropertiesExtensions
{
    public static RetroTypes RequiredRetroTyped(this RoomProperties roomProperties)
    {
        ArgumentNullException.ThrowIfNull(roomProperties);

        return Enum.Parse<RetroTypes>(roomProperties.RequiredRetroType(), ignoreCase: true);
    }
}