using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachPreview;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.AttachPreview;

internal sealed class AttachPreviewCommandHandler : IRequestHandler<AttachPreviewCommand, CommandResult>
{
    private readonly IDraftTaskForReviewRepository _repository;

    public AttachPreviewCommandHandler(IDraftTaskForReviewRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<CommandResult> Handle(AttachPreviewCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var draft = await _repository.GetById(command.DraftId, token);

        await _repository.Upsert(draft.WithPreviewMessage(command.MessageId), token);
        
        return CommandResult.Empty;
    }
}