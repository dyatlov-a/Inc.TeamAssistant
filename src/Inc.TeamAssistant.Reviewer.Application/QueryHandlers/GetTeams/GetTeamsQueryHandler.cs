using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetTeams;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetTeams;

internal sealed class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, GetTeamsResult>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamsQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
    }

    public async Task<GetTeamsResult> Handle(GetTeamsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        
        var teams = await _teamRepository.GetTeamNames(query.PersonId, query.ChatId, cancellationToken);
        var results = teams.Select(t => new TeamDto(t.Id, t.Name)).ToArray();

        return new GetTeamsResult(results);
    }
}