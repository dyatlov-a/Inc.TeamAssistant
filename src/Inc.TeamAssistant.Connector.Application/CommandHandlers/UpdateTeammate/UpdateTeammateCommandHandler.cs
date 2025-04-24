using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Commands.UpdateTeammate;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.CommandHandlers.UpdateTeammate;

internal sealed class UpdateTeammateCommandHandler : IRequestHandler<UpdateTeammateCommand>
{
    private readonly IPersonRepository _repository;
    private readonly ICurrentPersonResolver _personProvider;
    private readonly ITeamAccessor _teamAccessor;

    public UpdateTeammateCommandHandler(
        IPersonRepository repository,
        ICurrentPersonResolver personProvider,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personProvider = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task Handle(UpdateTeammateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var teammateKey = new TeammateKey(command.TeamId, command.PersonId);
        var currentPerson = _personProvider.GetCurrentPerson();
        
        await _teamAccessor.EnsureManagerAccess(command.TeamId, currentPerson.Id, token);
        
        var teammate = await teammateKey.Required(_repository.Find, token);

        if (command.HasLeaveUntil)
            teammate.ChangeLeaveUntil(command.LeaveUntil);
        if (command.CanFinalize.HasValue)
            teammate.ChangeCanFinalize(command.CanFinalize.Value);
        
        await _repository.Upsert(teammate, token);
    }
}