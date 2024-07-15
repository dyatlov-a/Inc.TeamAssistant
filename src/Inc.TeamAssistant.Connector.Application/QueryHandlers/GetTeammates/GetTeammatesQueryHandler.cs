using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetTeammates;

internal sealed class GetTeammatesQueryHandler : IRequestHandler<GetTeammatesQuery, GetTeammatesResult>
{
    private readonly ITeamReader _teamReader;
    private readonly ICurrentPersonResolver _currentPersonProvider;

    public GetTeammatesQueryHandler(ITeamReader teamReader, ICurrentPersonResolver currentPersonProvider)
    {
        _teamReader = teamReader ?? throw new ArgumentNullException(nameof(teamReader));
        _currentPersonProvider = currentPersonProvider ?? throw new ArgumentNullException(nameof(currentPersonProvider));
    }

    public async Task<GetTeammatesResult> Handle(GetTeammatesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _currentPersonProvider.GetCurrentPerson();
        var hasManagerAccess = await _teamReader.HasManagerAccess(query.TeamId, currentPerson.Id, token);
        var teammates = await _teamReader.GetTeammates(query.TeamId, token);

        return new GetTeammatesResult(hasManagerAccess, teammates);
    }
}