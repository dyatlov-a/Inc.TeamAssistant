using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;

public sealed record GetRetroAssessmentResult(int Value)
    : IWithEmpty<GetRetroAssessmentResult>
{
    public static GetRetroAssessmentResult Empty { get; } = new(Value: 0);
}