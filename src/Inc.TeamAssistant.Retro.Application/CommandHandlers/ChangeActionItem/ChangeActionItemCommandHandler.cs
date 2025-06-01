using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeActionItem.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.ChangeActionItem;

internal sealed class ChangeActionItemCommandHandler : IRequestHandler<ChangeActionItemCommand>
{
    private readonly IActionItemRepository _repository;
    private readonly IRetroEventSender _eventSender;

    public ChangeActionItemCommandHandler(IActionItemRepository repository, IRetroEventSender eventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(ChangeActionItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var item = command.Id.HasValue
            ? await command.Id.Value.Required(_repository.Find, token)
            : new ActionItem(Guid.CreateVersion7(), command.RetroItemId, DateTimeOffset.UtcNow);

        await _repository.Upsert(item.ChangeText(command.Text), token);

        await _eventSender.ActionItemChanged(command.TeamId, ActionItemConverter.ConvertTo(item));
    }
}