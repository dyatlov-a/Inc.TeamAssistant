using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetTeammates;

internal sealed class GetTeammatesQueryHandler : IRequestHandler<GetTeammatesQuery, GetTeammatesResult>
{
    private readonly ITeamReader _teamReader;

    public GetTeammatesQueryHandler(ITeamReader teamReader)
    {
        _teamReader = teamReader ?? throw new ArgumentNullException(nameof(teamReader));
    }

    public async Task<GetTeammatesResult> Handle(GetTeammatesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var teammates = await _teamReader.GetTeammates(query.TeamId, token);

        return new GetTeammatesResult(teammates);
    }
}