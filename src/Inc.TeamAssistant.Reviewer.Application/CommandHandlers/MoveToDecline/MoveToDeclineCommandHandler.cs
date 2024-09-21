using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline;

internal sealed class MoveToDeclineCommandHandler : IRequestHandler<MoveToDeclineCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IBotAccessor _botAccessor;

    public MoveToDeclineCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder,
        ITeamAccessor teamAccessor,
        IBotAccessor botAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task<CommandResult> Handle(MoveToDeclineCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await _repository.GetById(command.TaskId, token);
        var botContext = await _botAccessor.GetBotContext(taskForReview.BotId, token);
        
        if (!taskForReview.CanAccept())
            return CommandResult.Empty;
        
        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);

        taskForReview.Decline(DateTimeOffset.UtcNow, botContext.GetNotificationIntervals());

        var notifications = await _reviewMessageBuilder.Build(
            command.MessageContext.ChatMessage.MessageId,
            taskForReview,
            reviewer,
            owner,
            token);
        
        await _repository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}