using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeammate;

internal sealed class RemoveTeammateCommandHandler : IRequestHandler<RemoveTeammateCommand>
{
    private readonly IPersonRepository _repository;
    private readonly ICurrentPersonResolver _personProvider;
    private readonly ITeamAccessor _teamAccessor;

    public RemoveTeammateCommandHandler(
        IPersonRepository repository,
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

        var currentPerson = _personProvider.GetCurrentPerson();
        
        await _teamAccessor.EnsureManagerAccess(command.TeamId, currentPerson.Id, token);
        
        if (command.Exclude)
            await _repository.LeaveFromTeam(command.TeamId, command.PersonId, token);
        else
            await _repository.LeaveFromTeam(command.TeamId, command.PersonId, command.UntilDate, token);
    }
}