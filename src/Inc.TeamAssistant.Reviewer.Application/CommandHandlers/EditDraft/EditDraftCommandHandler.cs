using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.EditDraft;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.EditDraft;

internal sealed class EditDraftCommandHandler : IRequestHandler<EditDraftCommand, CommandResult>
{
    private readonly IDraftTaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;

    public EditDraftCommandHandler(IDraftTaskForReviewRepository repository, IReviewMessageBuilder reviewMessageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
    }

    public async Task<CommandResult> Handle(EditDraftCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var draft = await _repository.Find(
            command.MessageContext.ChatMessage.ChatId,
            command.MessageContext.ChatMessage.MessageId,
            token);

        if (draft is not null)
        {
            draft.WithDescription(command.Description);

            if (command.MessageContext.TargetPersonId.HasValue)
                draft.WithTargetPerson(command.MessageContext.TargetPersonId.Value);
            else
                draft.WithoutTargetPerson();
            
            var notification = await _reviewMessageBuilder.Build(draft, command.MessageContext.LanguageId, token);
            
            await _repository.Upsert(draft, token);
            
            return CommandResult.Build(notification);
        }

        return CommandResult.Empty;
    }
}