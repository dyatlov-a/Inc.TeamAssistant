using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AttachMessage;

internal sealed class AttachMessageCommandHandler : IRequestHandler<AttachMessageCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly ReviewCommentsProvider _commentsProvider;

    public AttachMessageCommandHandler(ITaskForReviewRepository repository, ReviewCommentsProvider commentsProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _commentsProvider = commentsProvider ?? throw new ArgumentNullException(nameof(commentsProvider));
    }

    public async Task<CommandResult> Handle(AttachMessageCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var messageType = Enum.Parse<MessageType>(command.MessageType);
        var task = await command.TaskId.Required(_repository.Find, token);
        
        await _repository.Upsert(task.AttachMessage(messageType, command.MessageId), token);
        
        _commentsProvider.Add(task);
        return CommandResult.Empty;
    }
}