using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Handlers;

internal sealed class LeaveTeamHandler : ILeaveTeamHandler
{
    private readonly ReassignReviewService _reassignReviewService;
    private readonly ITaskForReviewReader _taskForReviewReader;

    public LeaveTeamHandler(ReassignReviewService reassignReviewService, ITaskForReviewReader taskForReviewReader)
    {
        _reassignReviewService =
            reassignReviewService ?? throw new ArgumentNullException(nameof(reassignReviewService));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
    }

    public async Task<IEnumerable<NotificationMessage>> Handle(
        MessageContext messageContext,
        Guid teamId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        
        var notifications = new List<NotificationMessage>();
        var tasks = await _taskForReviewReader.GetTasksByPerson(
            teamId,
            messageContext.Person.Id,
            TaskForReviewStateRules.ActiveStates,
            token);

        foreach (var task in tasks)
            notifications.AddRange(await _reassignReviewService.ReassignReview(task.Id, chatMessage: null, token));

        return notifications;
    }
}