using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeRoomProperties;

public sealed record ChangeRoomPropertiesCommand(
    Guid RoomId,
    bool? IsFacilitator,
    Guid? TemplateId,
    TimeSpan? TimerDuration,
    int? VoteCount)
    : IRequest
{
    public static ChangeRoomPropertiesCommand ChangeFacilitator(Guid roomId)
    {
        return new ChangeRoomPropertiesCommand(
            roomId,
            IsFacilitator: true,
            TemplateId: null,
            TimerDuration: null,
            VoteCount: null);
    }
    
    public static ChangeRoomPropertiesCommand ChangeProperties(
        Guid roomId,
        Guid templateId,
        TimeSpan timerDuration,
        int voteCount)
    {
        return new ChangeRoomPropertiesCommand(
            roomId,
            IsFacilitator: true,
            templateId,
            timerDuration,
            voteCount);
    }
}