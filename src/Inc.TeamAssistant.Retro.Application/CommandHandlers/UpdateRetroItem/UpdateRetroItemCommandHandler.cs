using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.UpdateRetroItem;

internal sealed class UpdateRetroItemCommandHandler : IRequestHandler<UpdateRetroItemCommand>
{
    private readonly IRetroRepository _retroRepository;
    private readonly IPersonResolver _personResolver;

    public UpdateRetroItemCommandHandler(IRetroRepository retroRepository, IPersonResolver personResolver)
    {
        _retroRepository = retroRepository ?? throw new ArgumentNullException(nameof(retroRepository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }
    
    public async Task Handle(UpdateRetroItemCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var item = await command.Id.Required(_retroRepository.FindItem, token);

        item
            .CheckRights(person.Id)
            .ChangeText(command.Text);
        
        await _retroRepository.Upsert(item, token);
    }
}