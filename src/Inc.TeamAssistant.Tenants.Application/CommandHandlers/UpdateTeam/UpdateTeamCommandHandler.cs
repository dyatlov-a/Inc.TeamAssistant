using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Tenants.Application.Contracts;
using Inc.TeamAssistant.Tenants.Model.Commands.UpdateTeam;
using MediatR;

namespace Inc.TeamAssistant.Tenants.Application.CommandHandlers.UpdateTeam;

internal sealed class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand>
{
    private readonly ITenantRepository _repository;
    private readonly IPersonResolver _personResolver;

    public UpdateTeamCommandHandler(ITenantRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(UpdateTeamCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var person = _personResolver.GetCurrentPerson();
        var team = await command.Id.Required(_repository.FindTeam, token);
        
        team
            .CheckRights(person.Id)
            .ChangeName(command.Name);

        await _repository.Upsert(team, token);
    }
}