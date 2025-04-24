namespace Inc.TeamAssistant.Primitives.Features.Teams;

public interface IRemoveTeamHandler
{
    Task Handle(Guid teamId, CancellationToken token);
}