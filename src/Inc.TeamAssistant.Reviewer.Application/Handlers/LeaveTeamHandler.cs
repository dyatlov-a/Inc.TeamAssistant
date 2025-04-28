using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Teams;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;

namespace Inc.TeamAssistant.Reviewer.Application.Handlers;

internal sealed class LeaveTeamHandler : ILeaveTeamHandler
{
    private readonly ReassignReviewService _service;
    private readonly ITaskForReviewReader _reader;

    public LeaveTeamHandler(ReassignReviewService service, ITaskForReviewReader reader)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<IEnumerable<NotificationMessage>> Handle(TeammateKey key, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(key);

        var notifications = new List<NotificationMessage>();
        var tasks = await _reader.GetTasksByPerson(
            key.TeamId,
            key.PersonId,
            TaskForReviewStateRules.ActiveStates,
            token);

        foreach (var task in tasks)
        {
            var notificationsByTask = await _service.ReassignReview(task.Id, token);
            
            notifications.AddRange(notificationsByTask);
        }

        return notifications;
    }
}