using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Commands.SetVotes;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.CommandHandlers.SetVotes;

internal sealed class SetVotesCommandHandler : IRequestHandler<SetVotesCommand>
{
    private readonly IVoteStore _voteStore;
    private readonly IPersonResolver _personResolver;
    private readonly IRetroEventSender _eventSender;

    public SetVotesCommandHandler(
        IVoteStore voteStore,
        IPersonResolver personResolver,
        IRetroEventSender eventSender)
    {
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
    }

    public async Task Handle(SetVotesCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var votes = command.Votes
            .Select(v => new VoteTicket(v.ItemId, currentPerson.Id, v.Vote))
            .ToArray();
        var votesCount = votes.Sum(v => v.Vote);
        
        _voteStore.Set(command.TeamId, currentPerson.Id, votes);

        await _eventSender.VotesChanged(command.TeamId, currentPerson.Id, votesCount);
    }
}