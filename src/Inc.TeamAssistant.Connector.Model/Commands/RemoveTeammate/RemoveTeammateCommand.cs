using MediatR;

namespace Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;

public sealed record RemoveTeammateCommand(Guid TeamId, long PersonId)
    : IRequest;