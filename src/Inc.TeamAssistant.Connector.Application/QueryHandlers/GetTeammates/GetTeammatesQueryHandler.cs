using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetTeammates;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetTeammates;

internal sealed class GetTeammatesQueryHandler : IRequestHandler<GetTeammatesQuery, GetTeammatesResult>
{
    private readonly ITeamReader _reader;
    private readonly ICurrentPersonResolver _personProvider;
    private readonly ITeamAccessor _teamAccessor;

    public GetTeammatesQueryHandler(
        ITeamReader reader, 
        ICurrentPersonResolver personProvider,
        ITeamAccessor teamAccessor)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personProvider = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<GetTeammatesResult> Handle(GetTeammatesQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _personProvider.GetCurrentPerson();
        var hasManagerAccess = await _teamAccessor.HasManagerAccess(new(query.TeamId, currentPerson.Id), token);
        var teammates = await _reader.GetTeammates(query.TeamId, token);

        return new GetTeammatesResult(hasManagerAccess, teammates);
    }
}