using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class NotificationsService : BackgroundService
{
    private static readonly IReadOnlyCollection<(MessageId MessageId, string Command)> ReviewerCommands = new[]
    {
        (Messages.Reviewer_MoveToInProgress, CommandList.MoveToInProgress),
        (Messages.Reviewer_MoveToAccept, CommandList.Accept),
        (Messages.Reviewer_MoveToDecline, CommandList.Decline)
    };
    
    private readonly ITaskForReviewAccessor _accessor;
    private readonly IHolidayService _holidayService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITeamAccessor _teamAccessor;
    private readonly INotificationMessageSender _notificationMessageSender;
    private readonly WorkdayOptions _options;
    private readonly int _notificationsBatch;
    private readonly TimeSpan _notificationsDelay;

    public NotificationsService(
        ITaskForReviewAccessor accessor,
        IHolidayService holidayService,
        IServiceProvider serviceProvider,
        ITeamAccessor teamAccessor,
        INotificationMessageSender notificationMessageSender,
        WorkdayOptions options,
        int notificationsBatch,
        TimeSpan notificationsDelay)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _notificationMessageSender =
            notificationMessageSender ?? throw new ArgumentNullException(nameof(notificationMessageSender));
        _notificationsBatch = notificationsBatch;
        _notificationsDelay = notificationsDelay;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            using var scope = _serviceProvider.CreateScope();
            var translateProvider = scope.ServiceProvider.GetRequiredService<ITranslateProvider>();

            if (await IsWorkTime(now, token))
            {
                var tasksForNotifications = await _accessor.GetTasksForNotifications(
                    now,
                    TaskForReviewStateRules.ActiveStates,
                    _notificationsBatch,
                    token);

                foreach (var task in tasksForNotifications)
                {
                    task.SetNextNotificationTime(_options.NotificationInterval);

                    var message = task.State switch
                    {
                        TaskForReviewState.New or TaskForReviewState.InProgress => await CreateNeedReviewMessage(translateProvider, task, token),
                        TaskForReviewState.OnCorrection => await CreateMoveToNextRoundMessage(translateProvider, task, token),
                        _ => throw new ArgumentOutOfRangeException($"Value {task.State} OutOfRange for {nameof(TaskForReviewState)}")
                    };

                    await _notificationMessageSender.Send(task.TeamId, message, token);
                }

                await _accessor.Update(tasksForNotifications, token);
            }

            await Task.Delay(_notificationsDelay, token);
        }
    }

    private async Task<NotificationMessage> CreateNeedReviewMessage(
        ITranslateProvider translateProvider,
        TaskForReview task,
        CancellationToken token)
    {
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (task is null)
            throw new ArgumentNullException(nameof(task));

        var reviewer = await _teamAccessor.FindPerson(task.ReviewerId, token);
        if (!reviewer.HasValue)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.ReviewerId);

        var message = NotificationMessage.Create(
            reviewer.Value.Id,
            await translateProvider.Get(Messages.Reviewer_NeedReview, reviewer.Value.LanguageId, task.Description));

        foreach (var command in ReviewerCommands)
        {
            var text = await translateProvider.Get(command.MessageId, reviewer.Value.LanguageId);
            message.WithButton(new Button(text, $"{command.Command}{task.Id:N}"));
        }

        return message;
    }

    private async Task<NotificationMessage> CreateMoveToNextRoundMessage(
        ITranslateProvider translateProvider,
        TaskForReview task,
        CancellationToken token)
    {
        if (translateProvider is null)
            throw new ArgumentNullException(nameof(translateProvider));
        if (task is null)
            throw new ArgumentNullException(nameof(task));

        var owner = await _teamAccessor.FindPerson(task.OwnerId, token);
        if (!owner.HasValue)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.OwnerId);

        var message = NotificationMessage.Create(
            owner.Value.Id,
            await translateProvider.Get(Messages.Reviewer_ReviewDeclined, owner.Value.LanguageId, task.Description));
        message.WithButton(new Button(
            await translateProvider.Get(Messages.Reviewer_MoveToNextRound, owner.Value.LanguageId),
            $"{CommandList.MoveToNextRound}{task.Id:N}"));

        return message;
    }

    private async Task<bool> IsWorkTime(DateTimeOffset dateTimeOffset, CancellationToken token)
    {
        if (_options.WorkOnHoliday)
            return true;

        if (dateTimeOffset.TimeOfDay < _options.StartTimeUtc || dateTimeOffset.TimeOfDay >= _options.EndTimeUtc)
            return false;

        return await _holidayService.IsWorkday(DateOnly.FromDateTime(dateTimeOffset.DateTime), token);
    }
}