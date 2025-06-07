using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Queries.GetRetroAssessment;

public sealed record GetRetroAssessmentQuery(Guid SessionId)
    : IRequest<GetRetroAssessmentResult>;