using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.SetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetRetroState;

internal sealed class SetRetroStateCommandHandler : IRequestHandler<SetRetroStateCommand>
{
    private readonly IRetroEventSender _eventSender;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IVoteStore _voteStore;
    private readonly IOnlinePersonStore _onlinePersonStore;
    private readonly IRetroSessionReader _retroSessionReader;

    public SetRetroStateCommandHandler(
        IRetroEventSender eventSender,
        ITeamAccessor teamAccessor,
        IVoteStore voteStore,
        IOnlinePersonStore onlinePersonStore,
        IRetroSessionReader retroSessionReader)
    {
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
        _retroSessionReader = retroSessionReader ?? throw new ArgumentNullException(nameof(retroSessionReader));
    }

    public async Task Handle(SetRetroStateCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var person = await _teamAccessor.EnsurePerson(command.PersonId, token);
        var retroSession = await _retroSessionReader.FindSession(command.RoomId, RetroSessionStateRules.Active, token);
        var totalVote = retroSession is not null
            ? _voteStore.Get(retroSession.Id).Where(v => v.PersonId == person.Id).Sum(v => v.Vote)
            : 0;
        
        _onlinePersonStore.SetTicket(
            RoomId.CreateForRetro(command.RoomId),
            person,
            totalVote,
            command.Finished,
            command.HandRaised);

        await _eventSender.RetroStateChanged(
            command.RoomId,
            command.PersonId,
            command.Finished,
            command.HandRaised);
    }
}