using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.AddComment;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AddComment.Services;

internal sealed class AddCommentCommandCreator : ICommandCreator
{
    private readonly ReviewCommentsProvider _commentsProvider;

    public AddCommentCommandCreator(ReviewCommentsProvider commentsProvider)
    {
        _commentsProvider = commentsProvider ?? throw new ArgumentNullException(nameof(commentsProvider));
    }

    public IDialogCommand? TryCreate(
        string command,
        bool singleLineMode,
        MessageContext messageContext,
        CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(messageContext);
        ArgumentNullException.ThrowIfNull(teamContext);
        
        if (singleLineMode ||
            !messageContext.ChatMessage.ReplyToMessageId.HasValue ||
            string.IsNullOrWhiteSpace(messageContext.Text))
            return null;

        var messageId = messageContext.ChatMessage.ReplyToMessageId.Value;
        
        return _commentsProvider.Check(messageContext.ChatMessage.ChatId, messageId).HasValue
            ? new AddCommentCommand(messageContext, messageId, messageContext.Text)
            : null;
    }
}