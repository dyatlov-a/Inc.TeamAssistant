using MediatR;

namespace Inc.TeamAssistant.Reviewer.Model.Commands.RemoveTeammate;

public sealed record RemoveTeammateCommand(Guid TeamId, long PersonId)
    : IRequest;