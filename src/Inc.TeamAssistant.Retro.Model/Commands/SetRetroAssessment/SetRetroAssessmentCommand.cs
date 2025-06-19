using MediatR;

namespace Inc.TeamAssistant.Retro.Model.Commands.SetRetroAssessment;

public sealed record SetRetroAssessmentCommand(Guid SessionId, int Value)
    : IRequest;