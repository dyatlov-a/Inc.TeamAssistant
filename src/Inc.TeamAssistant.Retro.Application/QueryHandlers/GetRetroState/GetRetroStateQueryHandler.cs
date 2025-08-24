using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Application.Extensions;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroState;

internal sealed class GetRetroStateQueryHandler : IRequestHandler<GetRetroStateQuery, GetRetroStateResult>
{
    private readonly IRetroSessionReader _retroSessionReader;
    private readonly IPersonResolver _personResolver;
    private readonly IOnlinePersonStore _onlinePersonStore;
    private readonly IVoteStore _voteStore;
    private readonly ITimerService _timerService;
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly IRetroTemplateReader _retroTemplateReader;

    public GetRetroStateQueryHandler(
        IRetroSessionReader retroSessionReader,
        IPersonResolver personResolver,
        IOnlinePersonStore onlinePersonStore,
        IVoteStore voteStore,
        ITimerService timerService,
        IRoomPropertiesProvider propertiesProvider,
        IRetroTemplateReader retroTemplateReader)
    {
        _retroSessionReader = retroSessionReader ?? throw new ArgumentNullException(nameof(retroSessionReader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
        _timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _retroTemplateReader = retroTemplateReader ?? throw new ArgumentNullException(nameof(retroTemplateReader));
    }

    public async Task<GetRetroStateResult> Handle(GetRetroStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var states = RetroSessionStateRules.Active;
        var currentPerson = _personResolver.GetCurrentPerson();
        var onlinePersons = _onlinePersonStore.GetTickets(RoomId.CreateForRetro(query.RoomId));
        
        var session = await _retroSessionReader.FindSession(query.RoomId, states, token);
        var items = await _retroSessionReader.ReadRetroItems(query.RoomId, states, token);
        var actions = session is not null
            ? await _retroSessionReader.ReadActionItems(session.Id, token)
            : [];
        var properties = await _propertiesProvider.Get(query.RoomId, token);
        var retroTemplateId = session?.TemplateId ?? properties.RetroTemplateId;
        var columns = await _retroTemplateReader.GetColumns(retroTemplateId, token);
        
        var retroType = properties.RequiredRetroType();
        var retroProperties = new RetroPropertiesDto(
            properties.FacilitatorId,
            properties.TimerDuration,
            properties.VoteCount,
            properties.VoteByItemCount,
            retroType.ToString());
        var votes = session is not null
            ? _voteStore.Get(session.Id)
            : [];
        var votesByPerson = votes
            .Where(v => v.PersonId == currentPerson.Id)
            .ToDictionary(v => v.ItemId, v => v.Vote);
        
        var activeSession = session is not null
            ? RetroSessionConverter.ConvertTo(session)
            : null;
        var retroItems = items
            .Where(i => retroType != RetroTypes.Closed || i.OwnerId == currentPerson.Id)
            .Select(i => RetroItemConverter.ConvertFromReadModel(
                i,
                currentPerson.Id,
                session?.State,
                votesByPerson,
                retroType))
            .ToArray();
        var actionItems = actions.Select(ActionItemConverter.ConvertTo).ToArray();
        var retroColumns = columns
            .Select(c => new RetroColumnDto(
                c.Id,
                c.Name,
                c.Position,
                c.Color,
                c.Description))
            .ToArray();
        var currentTimer = _timerService.TryGetValue(query.RoomId);
        
        return new GetRetroStateResult(
            activeSession,
            retroItems,
            onlinePersons,
            actionItems,
            retroColumns,
            retroProperties,
            currentTimer);
    }
}