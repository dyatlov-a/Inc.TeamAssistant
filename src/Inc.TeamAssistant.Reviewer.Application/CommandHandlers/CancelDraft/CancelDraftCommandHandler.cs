using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.CancelDraft;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.CancelDraft;

internal sealed class CancelDraftCommandHandler : IRequestHandler<CancelDraftCommand, CommandResult>
{
    private readonly IDraftTaskForReviewRepository _repository;
    private readonly DraftTaskForReviewService _service;

    public CancelDraftCommandHandler(IDraftTaskForReviewRepository repository, DraftTaskForReviewService service)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<CommandResult> Handle(CancelDraftCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var draft = await command.DraftId.Required(_repository.Find, token);
        var notifications = await _service.Delete(
            draft.CheckRights(command.MessageContext.Person.Id),
            token);

        return CommandResult.Build(notifications.ToArray());
    }
}