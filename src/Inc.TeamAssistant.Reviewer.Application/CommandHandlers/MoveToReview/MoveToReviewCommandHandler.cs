using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
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
        ArgumentNullException.ThrowIfNull(command);

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
            taskForReview.DetectReviewer(teammates.Select(t => t.Id).ToArray(), lastReviewerId);
        }
        
        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);
        
        var taskForReviewMessage = await _messageBuilderService.BuildMessageNewTaskForReview(
            taskForReview,
            reviewer,
            owner,
            token);
        
        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(taskForReviewMessage);
    }
}