using Inc.TeamAssistant.Retro.Application.Common.Converters;
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

        var item = await _repository.Find(command.Id, token) ?? new ActionItem(
            command.Id,
            command.RetroItemId,
            DateTimeOffset.UtcNow);

        item.ChangeText(command.Text);
        
        if (!string.IsNullOrWhiteSpace(command.State))
            item.ChangeState(Enum.Parse<ActionItemState>(command.State, ignoreCase: true));
        
        await _repository.Upsert(item, token);

        if (command.TeamIdForNotify.HasValue)
            await _eventSender.ActionItemChanged(command.TeamIdForNotify.Value, ActionItemConverter.ConvertTo(item));
    }
}