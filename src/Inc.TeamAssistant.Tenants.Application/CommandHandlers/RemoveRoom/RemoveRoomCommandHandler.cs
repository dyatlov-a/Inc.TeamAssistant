using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.RemoveRoom;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.RemoveRoom;

internal sealed class RemoveRoomCommandHandler : IRequestHandler<RemoveRoomCommand>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IPersonResolver _personResolver;

    public RemoveRoomCommandHandler(ITenantRepository tenantRepository, IPersonResolver personResolver)
    {
        _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(RemoveRoomCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var room = await command.RoomId.Required(_tenantRepository.FindRoom, token);

        await _tenantRepository.Remove(room.CheckRights(person.Id), token);
    }
}