using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetCanStartReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetCanStartReview;

internal sealed class GetCanStartReviewQueryHandler : IRequestHandler<GetCanStartReviewQuery, GetCanStartReviewResult>
{
    private readonly ITeamRepository _teamRepository;

    public GetCanStartReviewQueryHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository ?? throw new ArgumentNullException(nameof(teamRepository));
    }

    public async Task<GetCanStartReviewResult> Handle(GetCanStartReviewQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        
        var currentTeam = await _teamRepository.Find(query.TeamId, cancellationToken);

        return new GetCanStartReviewResult(currentTeam?.CanStartReview());
    }
}