using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AttachMessage;

internal sealed class AttachMessageCommandHandler : IRequestHandler<AttachMessageCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;

    public AttachMessageCommandHandler(ITaskForReviewRepository taskForReviewRepository)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
    }

    public async Task<CommandResult> Handle(AttachMessageCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        
        taskForReview.AttachMessage(command.MessageId);
        
        await _taskForReviewRepository.Upsert(taskForReview, token);

        return CommandResult.Empty;
    }
}