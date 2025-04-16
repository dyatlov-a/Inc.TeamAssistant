using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AttachMessage;

internal sealed class AttachMessageCommandHandler : IRequestHandler<AttachMessageCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;

    public AttachMessageCommandHandler(ITaskForReviewRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CommandResult> Handle(AttachMessageCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var messageType = Enum.Parse<MessageType>(command.MessageType);
        var taskForReview = await command.TaskId.Required(_repository.Find, token);
        
        await _repository.Upsert(taskForReview.AttachMessage(messageType, command.MessageId), token);

        return CommandResult.Empty;
    }
}