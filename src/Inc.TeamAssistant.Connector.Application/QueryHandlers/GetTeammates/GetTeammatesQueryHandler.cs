using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetTeammates;

internal sealed class GetTeammatesQueryHandler : IRequestHandler<GetTeammatesQuery, GetTeammatesResult>
{
    private readonly ITeamReader _teamReader;
    private readonly ICurrentPersonResolver _currentPersonProvider;
    private readonly ITeamAccessor _teamAccessor;

    public GetTeammatesQueryHandler(
        ITeamReader teamReader, 
        ICurrentPersonResolver currentPersonProvider,
        ITeamAccessor teamAccessor)
    {
        _teamReader = teamReader ?? throw new ArgumentNullException(nameof(teamReader));
        _currentPersonProvider = currentPersonProvider ?? throw new ArgumentNullException(nameof(currentPersonProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<GetTeammatesResult> Handle(GetTeammatesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _currentPersonProvider.GetCurrentPerson();
        var hasManagerAccess = await _teamAccessor.HasManagerAccess(query.TeamId, currentPerson.Id, token);
        var teammates = await _teamReader.GetTeammates(query.TeamId, token);

        return new GetTeammatesResult(hasManagerAccess, teammates);
    }
}