using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Handlers;

internal sealed class RemoveTeamHandler : IRemoveTeamHandler
{
    private readonly ITaskForReviewRepository _repository;

    public RemoveTeamHandler(ITaskForReviewRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task Handle(Guid teamId, CancellationToken token)
    {
        var tasks = await _repository.GetAll(teamId, TaskForReviewStateRules.ActiveStates, token);

        foreach (var task in tasks)
            await _repository.Upsert(task.FinishRound(DateTimeOffset.UtcNow, hasComments: true), token);
    }
}