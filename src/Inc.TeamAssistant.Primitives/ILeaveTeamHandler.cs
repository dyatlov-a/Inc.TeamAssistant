namespace Inc.TeamAssistant.Primitives;

public interface ILeaveTeamHandler
{
    Task Handle(MessageContext messageContext, Guid teamId, CancellationToken token);
}