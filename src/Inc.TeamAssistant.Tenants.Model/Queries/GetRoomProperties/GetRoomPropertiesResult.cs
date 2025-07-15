using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Model.Common;

namespace Inc.TeamAssistant.Tenants.Model.Queries.GetRoomProperties;

public sealed record GetRoomPropertiesResult(
    RoomPropertiesDto Properties,
    bool IsFacilitator,
    TimeSpan? CurrentTimer)
    : IWithEmpty<GetRoomPropertiesResult>
{
    public static GetRoomPropertiesResult Empty { get; } = new(
        Properties: new RoomPropertiesDto(
            FacilitatorId: null,
            RetroTemplateId: Guid.Empty,
            SurveyTemplateId: Guid.Empty,
            TimerDuration: TimeSpan.Zero,
            VoteCount: 0,
            VoteByItemCount: 0,
            RetroType: string.Empty),
        IsFacilitator: false,
        CurrentTimer: null);
}