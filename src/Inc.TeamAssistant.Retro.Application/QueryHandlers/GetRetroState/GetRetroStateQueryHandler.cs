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
        
        var session = await _reader.FindSession(query.TeamId, states, token);
        var items = await _reader.ReadRetroItems(query.TeamId, states, token);
        var actions = session is not null
            ? await _reader.ReadActionItems(session.Id, token)
            : [];
        
        var votesByPerson = votes
            .Where(v => v.PersonId == currentPerson.Id)
            .ToDictionary(v => v.ItemId, v => v.Vote);
        var totalVotes = votes
            .GroupBy(v => v.PersonId, v => v.Vote)
            .ToDictionary(v => v.Key, v => v.Sum(i => i));
        var participants = onlinePersons
            .Select(op => new ParticipantDto(op, totalVotes.GetValueOrDefault(op.Id, 0)))
            .ToArray();
        var activeSession = session is not null
            ? RetroSessionConverter.ConvertTo(session)
            : null;
        var retroItems = items
            .Select(i => RetroItemConverter.ConvertFromReadModel(
                i,
                currentPerson.Id,
                session?.State,
                votesByPerson))
            .ToArray();
        var actionItems = actions
            .Select(ActionItemConverter.ConvertTo)
            .ToArray();
        
        return new GetRetroStateResult(activeSession, retroItems, participants, actionItems);
    }
}