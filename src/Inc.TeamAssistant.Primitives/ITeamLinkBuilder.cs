namespace Inc.TeamAssistant.Primitives;

public interface ITeamLinkBuilder
{
    string BuildLinkForConnect(string botName, Guid teamId);

    Task<(string TeamName, string Link, string Code)> GenerateTeamConnector(Guid teamId, CancellationToken token);
}