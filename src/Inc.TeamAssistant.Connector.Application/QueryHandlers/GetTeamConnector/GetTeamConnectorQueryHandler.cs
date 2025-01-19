using Inc.TeamAssistant.Connector.Model.Queries.GetTeamConnector;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetTeamConnector;

public sealed class GetTeamConnectorQueryHandler : IRequestHandler<GetTeamConnectorQuery, GetTeamConnectorResult>
{
    private readonly ITeamLinkBuilder _teamLinkBuilder;

    public GetTeamConnectorQueryHandler(ITeamLinkBuilder teamLinkBuilder)
    {
        _teamLinkBuilder = teamLinkBuilder ?? throw new ArgumentNullException(nameof(teamLinkBuilder));
    }

    public async Task<GetTeamConnectorResult> Handle(GetTeamConnectorQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var teamConnector = await _teamLinkBuilder.GenerateTeamConnector(
            query.TeamId,
            query.Foreground,
            query.Background,
            token);

        return new GetTeamConnectorResult(
            teamConnector.TeamName,
            teamConnector.Link,
            teamConnector.Code);
    }
}