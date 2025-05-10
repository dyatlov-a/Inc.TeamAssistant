using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.RemoveTeam;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.RemoveTeam;

internal sealed class RemoveTeamCommandHandler : IRequestHandler<RemoveTeamCommand>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IPersonResolver _personResolver;

    public RemoveTeamCommandHandler(ITenantRepository tenantRepository, IPersonResolver personResolver)
    {
        _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(RemoveTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var team = await command.TeamId.Required(_tenantRepository.FindTeam, token);

        await _tenantRepository.Remove(team.CheckRights(person.Id), token);
    }
}