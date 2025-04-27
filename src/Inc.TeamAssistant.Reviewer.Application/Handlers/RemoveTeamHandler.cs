using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Handlers;

internal sealed class RemoveTeamHandler : IRemoveTeamHandler
{
    private readonly ITaskForReviewReader _reader;
    private readonly ITaskForReviewRepository _repository;
    private readonly ReviewCommentsProvider _commentsProvider;

    public RemoveTeamHandler(
        ITaskForReviewReader reader,
        ITaskForReviewRepository repository,
        ReviewCommentsProvider commentsProvider)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _commentsProvider = commentsProvider ?? throw new ArgumentNullException(nameof(commentsProvider));
    }

    public async Task Handle(Guid teamId, CancellationToken token)
    {
        var tasks = await _reader.GetAll(TaskForReviewStateRules.ActiveStates, teamId, token);

        foreach (var task in tasks)
        {
            await _repository.Upsert(task.FinishRound(DateTimeOffset.UtcNow), token);
            
            _commentsProvider.Remove(task);
        }
    }
}