using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveRetroItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.RemoveRetroItem;

internal sealed class RemoveRetroItemCommandHandler : IRequestHandler<RemoveRetroItemCommand>
{
    private readonly IRetroItemRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public RemoveRetroItemCommandHandler(
        IRetroItemRepository repository,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }
    
    public async Task Handle(RemoveRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var item = await command.Id.Required(_repository.Find, token);

        await _repository.Remove(item.CheckCanRemove(person.Id), token);

        await _eventSender.RetroItemRemoved(item.TeamId, item.Id);
    }
}