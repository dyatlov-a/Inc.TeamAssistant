using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.AddComment;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AddComment;

internal sealed class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommandResult>
{
    private readonly ReviewCommentsProvider _commentsProvider;
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;

    public AddCommentCommandHandler(
        ReviewCommentsProvider commentsProvider,
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder)
    {
        _commentsProvider = commentsProvider ?? throw new ArgumentNullException(nameof(commentsProvider));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
    }

    public async Task<CommandResult> Handle(AddCommentCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var authorId = command.MessageContext.Person.Id;
        var taskId = _commentsProvider.Check(command.MessageContext.ChatMessage.ChatId, command.ReplyToMessageId);
        if (taskId.HasValue)
        {
            var task = await taskId.Value.Required(_repository.Find, token);
            if (!task.HasRightsForComments(authorId))
                throw new TeamAssistantUserException(Messages.Connector_HasNoRights);

            if (task.CanMakeDecision())
            {
                await _repository.Upsert(task.AddComment(DateTimeOffset.UtcNow, command.Comment, authorId), token);

                var messageForDelete = new ChatMessage(
                    command.MessageContext.ChatMessage.ChatId,
                    command.MessageContext.ChatMessage.MessageId);
                var notification = await _reviewMessageBuilder.BuildForTeam(task, token);
                return CommandResult.Build(notification, NotificationMessage.Delete(messageForDelete));
            }
        }

        return CommandResult.Empty;
    }
}