using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Handlers;

internal sealed class RemoveTeamHandler : IRemoveTeamHandler
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;

    public RemoveTeamHandler(ITaskForReviewRepository taskForReviewRepository)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
    }

    public async Task Handle(MessageContext messageContext, Guid teamId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        
        var tasks = await _taskForReviewRepository.Get(teamId, TaskForReviewStateRules.ActiveStates, token);

        foreach (var task in tasks)
        {
            task.MoveToArchive();
            
            await _taskForReviewRepository.Upsert(task, token);
        }
    }
}