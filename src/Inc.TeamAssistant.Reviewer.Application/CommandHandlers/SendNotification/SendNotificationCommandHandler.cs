using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.SendNotification;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.SendNotification;

internal sealed class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, CommandResult>
{
    private static readonly IReadOnlyCollection<(MessageId MessageId, string Command)> ReviewerCommands = new[]
    {
        (Messages.Reviewer_MoveToInProgress, CommandList.MoveToInProgress),
        (Messages.Reviewer_MoveToAccept, CommandList.Accept),
        (Messages.Reviewer_MoveToDecline, CommandList.Decline)
    };
    
    private readonly ITaskForReviewRepository _repository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ReviewerOptions _options;
    private readonly ITranslateProvider _translateProvider;

    public SendNotificationCommandHandler(
        ITaskForReviewRepository repository,
        ITeamAccessor teamAccessor,
        ReviewerOptions options,
        ITranslateProvider translateProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<CommandResult> Handle(SendNotificationCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var task = await _repository.GetById(command.TaskId, token);

        task.SetNextNotificationTime(_options.NotificationInterval);

        var notifications = task.State switch
        {
            TaskForReviewState.New or TaskForReviewState.InProgress => new[]
            {
                await CreateNeedReviewMessage(_translateProvider, task, token)
            },
            TaskForReviewState.OnCorrection => new[]
            {
                await CreateMoveToNextRoundMessage(_translateProvider, task, token)
            },
            _ => Array.Empty<NotificationMessage>()
        };

        await _repository.Upsert(task, token);

        return CommandResult.Build(notifications);
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
}