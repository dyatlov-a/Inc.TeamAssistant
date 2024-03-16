using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
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
        
        var targetTeam = command.MessageContext.FindTeam(command.TeamId);
        if (targetTeam is null)
            throw new TeamAssistantUserException(Messages.Connector_TeamNotFound, command.TeamId);
        
        var teammates = await _teamAccessor.GetTeammates(command.TeamId, token);
        if (!teammates.Any())
            throw new TeamAssistantUserException(Messages.Reviewer_TeamWithoutUsers, command.TeamId);

        var taskForReview = new TaskForReview(
            command.MessageContext.BotId,
            command.TeamId,
            Enum.Parse<NextReviewerType>(command.Strategy),
            command.MessageContext.PersonId,
            targetTeam.ChatId,
            command.Description);

        if (command.MessageContext.TargetPersonId.HasValue)
            taskForReview.SetConcreteReviewer(command.MessageContext.TargetPersonId.Value);
        else
        {
            var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(command.TeamId, token);
            taskForReview.DetectReviewer(teammates.Select(t => t.PersonId).ToArray(), lastReviewerId);
        }
        
        var taskForReviewMessage = await _messageBuilderService.NewTaskForReviewBuild(
            command.MessageContext.LanguageId,
            taskForReview,
            token);
        
        var notification = NotificationMessage
            .Create(targetTeam.ChatId, taskForReviewMessage.Text)
            .AttachPerson(taskForReviewMessage.AttachedPersonId);
        notification.AddHandler((c, p) => new AttachMessageCommand(c, taskForReview.Id, int.Parse(p)));
        
        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notification);
    }
}