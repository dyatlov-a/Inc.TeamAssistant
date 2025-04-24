using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeammate;

internal sealed class RemoveTeammateCommandHandler : IRequestHandler<RemoveTeammateCommand>
{
    private readonly ITeammateRepository _repository;
    private readonly ICurrentPersonResolver _personProvider;
    private readonly ITeamAccessor _teamAccessor;

    public RemoveTeammateCommandHandler(
        ITeammateRepository repository,
        ICurrentPersonResolver personProvider,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personProvider = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(RemoveTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var teammateKey = new TeammateKey(command.TeamId, command.PersonId);
        var currentPerson = _personProvider.GetCurrentPerson();
        
        await _teamAccessor.EnsureManagerAccess(command.TeamId, currentPerson.Id, token);
        
        await _repository.RemoveFromTeam(teammateKey, token);
    }
}