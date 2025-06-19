using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateRoom;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.UpdateRoom;

internal sealed class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand>
{
    private readonly ITenantRepository _repository;
    private readonly IPersonResolver _personResolver;

    public UpdateRoomCommandHandler(ITenantRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(UpdateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var room = await command.Id.Required(_repository.FindRoom, token);
        
        room
            .CheckRights(person.Id)
            .ChangeName(command.Name);

        await _repository.Upsert(room, token);
    }
}