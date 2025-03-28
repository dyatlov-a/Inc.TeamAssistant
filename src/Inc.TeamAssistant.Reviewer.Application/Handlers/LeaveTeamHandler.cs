using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Handlers;
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

    public async Task<IEnumerable<NotificationMessage>> Handle(
        MessageContext messageContext,
        Guid teamId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageContext);
        
        var tasks = await _reader.GetTasksByPerson(
            teamId,
            messageContext.Person.Id,
            TaskForReviewStateRules.ActiveStates,
            token);
        var notifications = new List<NotificationMessage>();

        foreach (var task in tasks)
        {
            var notificationsByTask = await _service.ReassignReview(
                messageContext.ChatMessage.MessageId,
                task.Id,
                messageContext.Bot,
                token);
            
            notifications.AddRange(notificationsByTask);
        }

        return notifications;
    }
}