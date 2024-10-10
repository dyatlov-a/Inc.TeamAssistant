using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Commands.RemoveTeammate;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.RemoveTeammate;

internal sealed class RemoveTeammateCommandHandler : IRequestHandler<RemoveTeammateCommand>
{
    private readonly IPersonRepository _personRepository;
    private readonly ICurrentPersonResolver _currentPersonProvider;
    private readonly ITeamAccessor _teamAccessor;

    public RemoveTeammateCommandHandler(
        IPersonRepository personRepository,
        ICurrentPersonResolver currentPersonProvider,
        ITeamAccessor teamAccessor)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _currentPersonProvider = currentPersonProvider ?? throw new ArgumentNullException(nameof(currentPersonProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(RemoveTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _currentPersonProvider.GetCurrentPerson();
        var hasManagerAccess = await _teamAccessor.HasManagerAccess(command.TeamId, currentPerson.Id, token);

        if (!hasManagerAccess)
            throw new ApplicationException(
                $"User {currentPerson.DisplayName} has not rights to remove teammate from team {command.TeamId}");
        
        if (command.Exclude)
            await _personRepository.LeaveFromTeam(command.TeamId, command.PersonId, token);
        else
            await _personRepository.LeaveFromTeam(command.TeamId, command.PersonId, command.UntilDate, token);
    }
}