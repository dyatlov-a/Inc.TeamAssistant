using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;

public sealed record GetRetroAssessmentResult(Guid RoomId, int Value)
    : IWithEmpty<GetRetroAssessmentResult>
{
    public static GetRetroAssessmentResult Empty { get; } = new(RoomId: Guid.Empty, Value: 0);
}