using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;

public sealed record ChangeRetroPropertiesCommand(
    Guid RoomId,
    bool? IsFacilitator,
    Guid? TemplateId,
    TimeSpan? TimerDuration,
    int? VoteCount)
    : IRequest
{
    public static ChangeRetroPropertiesCommand ChangeFacilitator(Guid roomId)
    {
        return new ChangeRetroPropertiesCommand(
            roomId,
            IsFacilitator: true,
            TemplateId: null,
            TimerDuration: null,
            VoteCount: null);
    }
    
    public static ChangeRetroPropertiesCommand ChangeProperties(
        Guid roomId,
        Guid templateId,
        TimeSpan timerDuration,
        int voteCount)
    {
        return new ChangeRetroPropertiesCommand(
            roomId,
            IsFacilitator: true,
            templateId,
            timerDuration,
            voteCount);
    }
}