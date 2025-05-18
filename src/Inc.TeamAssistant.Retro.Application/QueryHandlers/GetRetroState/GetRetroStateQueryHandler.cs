using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroState;

internal sealed class GetRetroStateQueryHandler : IRequestHandler<GetRetroStateQuery, GetRetroStateResult>
{
    private readonly IRetroReader _reader;
    private readonly IPersonResolver _personResolver;

    public GetRetroStateQueryHandler(IRetroReader reader, IPersonResolver personResolver)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetRetroStateResult> Handle(GetRetroStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var states = RetroSessionStateRules.Active;
        var person = _personResolver.GetCurrentPerson();
        var retroSession = await _reader.FindSession(query.TeamId, states, token);
        var retroItems = await _reader.ReadItems(query.TeamId, token);
        var votes = await _reader.ReadVotes(query.TeamId, states, token);
        
        var votesLookup = votes
            .Where(v => v.PersonId == person.Id)
            .SelectMany(v => v.Votes)
            .ToDictionary(v => v.ItemId, v => v.Vote);
        var retroSessionResult = retroSession is not null
            ? RetroSessionConverter.ConvertTo(retroSession)
            : null;
        var itemsResult = retroItems
            .Select(i => RetroItemConverter.ConvertTo(i, votesLookup))
            .ToArray();
        
        return new GetRetroStateResult(retroSessionResult, itemsResult);
    }
}