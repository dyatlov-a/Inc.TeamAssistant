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
    private readonly ITimerService _timerService;
    private readonly IRetroStage _retroStage;
    private readonly IRetroPropertiesProvider _propertiesProvider;

    public GetRetroStateQueryHandler(
        IRetroReader reader,
        IPersonResolver personResolver,
        IOnlinePersonStore onlinePersonStore,
        IVoteStore voteStore,
        ITimerService timerService,
        IRetroStage retroStage,
        IRetroPropertiesProvider propertiesProvider)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
        _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
        _retroStage = retroStage ?? throw new ArgumentNullException(nameof(retroStage));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    }

    public async Task<GetRetroStateResult> Handle(GetRetroStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var states = RetroSessionStateRules.Active;
        var currentPerson = _personResolver.GetCurrentPerson();
        var onlinePersons = _onlinePersonStore.GetPersons(query.RoomId);
        
        var session = await _reader.FindSession(query.RoomId, states, token);
        var items = await _reader.ReadRetroItems(query.RoomId, states, token);
        var actions = session is not null
            ? await _reader.ReadActionItems(session.Id, token)
            : [];
        var properties = await _propertiesProvider.Get(query.RoomId, token);

        var votes = session is not null
            ? _voteStore.Get(session.Id)
            : [];
        var votesByPerson = votes
            .Where(v => v.PersonId == currentPerson.Id)
            .ToDictionary(v => v.ItemId, v => v.Vote);
        var totalVotes = votes
            .GroupBy(v => v.PersonId, v => v.Vote)
            .ToDictionary(v => v.Key, v => v.Sum(i => i));
        var retroStage = _retroStage.Get(query.RoomId);
        var finishedLookup = retroStage.ToDictionary(s => s.PersonId, s => s.Finished);
        var handRaisedLookup = retroStage.ToDictionary(s => s.PersonId, s => s.HandRaised);
        
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
        var participants = onlinePersons
            .Select(op => new RetroParticipantDto(
                op,
                totalVotes.GetValueOrDefault(op.Id, 0),
                finishedLookup.GetValueOrDefault(op.Id, false),
                handRaisedLookup.GetValueOrDefault(op.Id, false)))
            .ToArray();
        var actionItems = actions
            .Select(ActionItemConverter.ConvertTo)
            .ToArray();
        var currentTimer = _timerService.TryGetValue(query.RoomId);
        
        return new GetRetroStateResult(
            activeSession,
            retroItems,
            participants,
            actionItems,
            currentTimer,
            properties.FacilitatorId);
    }
}