using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Domain;
using Inc.TeamAssistant.Tenants.Model.Commands.CreateTeam;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.CreateTeam;

internal sealed class CreateTeamCommandHandler
    : IRequestHandler<CreateTeamCommand, CreateTeamResult>
{
    private readonly ITenantRepository _repository;
    private readonly IPersonResolver _personResolver;

    public CreateTeamCommandHandler(ITenantRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }
    
    public async Task<CreateTeamResult> Handle(CreateTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var tenant = await _repository.FindTenant(person.Id, token);
        var team = new Team(
            Guid.CreateVersion7(),
            command.Name,
            person.Id,
            tenant ?? new Tenant(Guid.CreateVersion7(), command.Name, person.Id));

        await _repository.Upsert(team, token);

        return new(team.Id);
    }
}