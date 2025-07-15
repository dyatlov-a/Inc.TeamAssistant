using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Tenants.Model.Common;

namespace Inc.TeamAssistant.Tenants.Application.Common;

internal static class RoomPropertiesConverter
{
    private static readonly Guid DefaultSurveyTemplateId = Guid.Parse("6c9b2eef-b7ce-4e13-b866-1a0cd743c6b3");
    private static readonly TimeSpan DefaultTimerDuration = TimeSpan.FromMinutes(10);
    private static readonly int DefaultVoteCount = 5;
    private static readonly int DefaultVoteByItemCount = 2;
    
    public static RoomPropertiesDto ConvertTo(RoomProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        return new RoomPropertiesDto(
            properties.FacilitatorId,
            properties.RequiredRetroTemplateId(),
            properties.SurveyTemplateId ?? DefaultSurveyTemplateId,
            properties.TimerDuration ?? DefaultTimerDuration,
            properties.VoteCount ?? DefaultVoteCount,
            properties.VoteByItemCount ?? DefaultVoteByItemCount,
            properties.RequiredRetroType());
    }
}