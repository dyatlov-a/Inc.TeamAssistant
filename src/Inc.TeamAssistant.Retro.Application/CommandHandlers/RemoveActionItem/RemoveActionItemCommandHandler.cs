using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveActionItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.RemoveActionItem;

internal sealed class RemoveActionItemCommandHandler : IRequestHandler<RemoveActionItemCommand>
{
    private readonly IActionItemRepository _repository;
    private readonly IRetroEventSender _eventSender;

    public RemoveActionItemCommandHandler(IActionItemRepository repository, IRetroEventSender eventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(RemoveActionItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var item = await _repository.Find(command.Id, token);

        if (item is not null)
            await _repository.Remove(item, token);
        
        await _eventSender.ActionItemRemoved(command.RoomId, command.Id);
    }
}