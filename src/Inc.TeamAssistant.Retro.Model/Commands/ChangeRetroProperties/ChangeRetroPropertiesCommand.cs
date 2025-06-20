using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.ChangeRetroProperties;

public sealed record ChangeRetroPropertiesCommand(
    Guid RoomId,
    bool? IsFacilitator,
    Guid? TemplateId,
    TimeSpan? TimerDuration,
    int? VoteCount,
    string? RetroType)
    : IRequest
{
    public static ChangeRetroPropertiesCommand ChangeFacilitator(Guid roomId)
    {
        return new ChangeRetroPropertiesCommand(
            roomId,
            IsFacilitator: true,
            TemplateId: null,
            TimerDuration: null,
            VoteCount: null,
            RetroType: string.Empty);
    }
    
    public static ChangeRetroPropertiesCommand ChangeProperties(
        Guid roomId,
        Guid templateId,
        TimeSpan timerDuration,
        int voteCount,
        string retroType)
    {
        return new ChangeRetroPropertiesCommand(
            roomId,
            IsFacilitator: true,
            templateId,
            timerDuration,
            voteCount,
            retroType);
    }
}