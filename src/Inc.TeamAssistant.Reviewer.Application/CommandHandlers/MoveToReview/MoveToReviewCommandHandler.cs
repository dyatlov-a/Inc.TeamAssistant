using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToReview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToReview;

internal sealed class MoveToReviewCommandHandler : IRequestHandler<MoveToReviewCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITeamAccessor _teamAccessor;

    public MoveToReviewCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        ITeamAccessor teamAccessor)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<CommandResult> Handle(MoveToReviewCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var teammates = await _teamAccessor.GetTeammates(command.TeamId, token);
        var targetTeam = command.MessageContext.FindTeam(command.TeamId);
        if (targetTeam is null)
            throw new ApplicationException($"Team {command.TeamId} was not found.");
        
        var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(command.TeamId, token);
        var taskForReview = new TaskForReview(
                command.TeamId,
                Enum.Parse<NextReviewerType>(command.Strategy),
                command.MessageContext.PersonId,
                targetTeam.ChatId,
                command.Description)
            .DetectReviewer(teammates.Select(t => t.PersonId).ToArray(), lastReviewerId);
        
        var taskForReviewMessage = await _messageBuilderService.NewTaskForReviewBuild(
            command.MessageContext.LanguageId,
            taskForReview,
            token);
        
        var notification = NotificationMessage.Create(targetTeam.ChatId, taskForReviewMessage);
        notification.AddHandler((c, id) => new AttachMessageCommand(command.MessageContext, taskForReview.Id, id));
        
        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notification);
    }
}