using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroState;

internal sealed class GetRetroStateQueryHandler : IRequestHandler<GetRetroStateQuery, GetRetroStateResult>
{
    private readonly IRetroReader _reader;
    private readonly IPersonResolver _personResolver;
    private readonly IOnlinePersonStore _onlinePersonStore;
    private readonly IVoteStore _voteStore;

    public GetRetroStateQueryHandler(
        IRetroReader reader,
        IPersonResolver personResolver,
        IOnlinePersonStore onlinePersonStore,
        IVoteStore voteStore)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
    }

    public async Task<GetRetroStateResult> Handle(GetRetroStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var states = RetroSessionStateRules.Active;
        var currentPerson = _personResolver.GetCurrentPerson();
        var votes = _voteStore.Get(query.TeamId, currentPerson.Id);
        var onlinePersons = _onlinePersonStore.GetPersons(query.TeamId);
        
        var activeSession = await _reader.FindSession(query.TeamId, states, token);
        var retroItems = await _reader.ReadItems(query.TeamId, token);
        
        var votesByPerson = votes
            .Where(v => v.PersonId == currentPerson.Id)
            .ToDictionary(v => v.ItemId, v => v.Vote);
        var totalVotes = votes
            .GroupBy(v => v.PersonId, v => v.Vote)
            .ToDictionary(v => v.Key, v => v.Sum(i => i));
        var participants = onlinePersons
            .Select(op => new ParticipantDto(op, totalVotes.GetValueOrDefault(op.Id, 0)))
            .ToArray();
        var session = activeSession is not null
            ? RetroSessionConverter.ConvertTo(activeSession)
            : null;
        var items = retroItems
            .Select(i => RetroItemConverter.ConvertFromReadModel(
                i,
                currentPerson.Id,
                activeSession?.State,
                votesByPerson))
            .ToArray();
        
        return new GetRetroStateResult(session, items, participants);
    }
}