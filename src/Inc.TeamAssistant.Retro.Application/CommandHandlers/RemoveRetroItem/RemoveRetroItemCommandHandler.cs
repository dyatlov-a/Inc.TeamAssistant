using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.RemoveRetroItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.RemoveRetroItem;

internal sealed class RemoveRetroItemCommandHandler : IRequestHandler<RemoveRetroItemCommand>
{
    private readonly IRetroRepository _retroRepository;
    private readonly IPersonResolver _personResolver;

    public RemoveRetroItemCommandHandler(IRetroRepository retroRepository, IPersonResolver personResolver)
    {
        _retroRepository = retroRepository ?? throw new ArgumentNullException(nameof(retroRepository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }
    
    public async Task Handle(RemoveRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var item = await command.Id.Required(_retroRepository.FindItem, token);

        await _retroRepository.Remove(item.CheckRights(person.Id), token);
    }
}