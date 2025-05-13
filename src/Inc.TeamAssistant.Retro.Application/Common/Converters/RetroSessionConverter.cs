using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroSessionConverter
{
    public static RetroSessionDto ConvertTo(RetroSession retroSession)
    {
        ArgumentNullException.ThrowIfNull(retroSession);

        return new RetroSessionDto(
            retroSession.Id,
            retroSession.TeamId,
            retroSession.Created,
            retroSession.State.ToString(),
            retroSession.FacilitatorId);
    }
}