using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateRoom;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.CreateRoom;

internal sealed class CreateRoomCommandHandler
    : IRequestHandler<CreateRoomCommand, CreateRoomResult>
{
    private readonly ITenantRepository _repository;
    private readonly IPersonResolver _personResolver;

    public CreateRoomCommandHandler(ITenantRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }
    
    public async Task<CreateRoomResult> Handle(CreateRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var tenant = await _repository.FindTenant(person.Id, token);
        var room = new Room(
            Guid.CreateVersion7(),
            command.Name,
            person.Id,
            RoomProperties.Default,
            tenant ?? new Tenant(Guid.CreateVersion7(), command.Name, person.Id));

        await _repository.Upsert(room, token);

        return new(room.Id);
    }
}