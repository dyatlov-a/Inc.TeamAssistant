using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Features.Rooms;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Application.Extensions;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroState;

internal sealed class GetRetroStateQueryHandler : IRequestHandler<GetRetroStateQuery, GetRetroStateResult>
{
    private readonly IRetroReader _reader;
    private readonly IPersonResolver _personResolver;
    private readonly IVoteStore _voteStore;
    private readonly IRetroTemplateReader _retroTemplateReader;
    private readonly IRoomPropertiesProvider _propertiesProvider;

    public GetRetroStateQueryHandler(
        IRetroReader reader,
        IPersonResolver personResolver,
        IVoteStore voteStore,
        IRetroTemplateReader retroTemplateReader,
        IRoomPropertiesProvider propertiesProvider)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _voteStore = voteStore ?? throw new ArgumentNullException(nameof(voteStore));
        _retroTemplateReader = retroTemplateReader ?? throw new ArgumentNullException(nameof(retroTemplateReader));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
    }

    public async Task<GetRetroStateResult> Handle(GetRetroStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var states = RetroSessionStateRules.Active;
        var currentPerson = _personResolver.GetCurrentPerson();
        
        var session = await _reader.FindSession(query.RoomId, states, token);
        var items = await _reader.ReadRetroItems(query.RoomId, states, token);
        var actions = session is not null
            ? await _reader.ReadActionItems(session.Id, token)
            : [];
        var roomProperties = await _propertiesProvider.Get(query.RoomId, token);
        var retroType = roomProperties.RequiredRetroTyped();
        var columns = await _retroTemplateReader.GetColumns(roomProperties.RequiredRetroTemplateId(), token);

        var votes = session is not null
            ? _voteStore.Get(session.Id)
            : [];
        var votesByPerson = votes
            .Where(v => v.PersonId == currentPerson.Id)
            .ToDictionary(v => v.ItemId, v => v.Vote);
        var totalVotes = votes
            .GroupBy(v => v.PersonId, v => v.Vote)
            .ToDictionary(v => v.Key, v => v.Sum(i => i));
        
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
        
        return new GetRetroStateResult(activeSession, retroItems, actionItems, retroColumns);
    }
}