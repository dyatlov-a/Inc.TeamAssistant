using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroState;

internal sealed class GetRetroStateQueryHandler : IRequestHandler<GetRetroStateQuery, GetRetroStateResult>
{
    private readonly IRetroReader _reader;

    public GetRetroStateQueryHandler(IRetroReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetRetroStateResult> Handle(GetRetroStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var retroSession = await _reader.FindSession(query.TeamId, RetroSessionStateRules.Active, token);
        var retroItems = await _reader.ReadItems(query.TeamId, token);

        var activeSession = retroSession is not null
            ? RetroSessionConverter.ConvertTo(retroSession)
            : null;
        var itemsResult = retroItems
            .Select(RetroItemConverter.ConvertTo)
            .ToArray();
        
        return new GetRetroStateResult(activeSession, itemsResult);
    }
}