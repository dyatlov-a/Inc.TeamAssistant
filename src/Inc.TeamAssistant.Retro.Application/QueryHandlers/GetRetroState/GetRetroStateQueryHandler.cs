using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroState;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroState;

internal sealed class GetRetroStateQueryHandler : IRequestHandler<GetRetroStateQuery, GetRetroStateResult>
{
    private readonly IRetroReader _retroReader;

    public GetRetroStateQueryHandler(IRetroReader retroReader)
    {
        _retroReader = retroReader ?? throw new ArgumentNullException(nameof(retroReader));
    }

    public async Task<GetRetroStateResult> Handle(GetRetroStateQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var retroSession = await _retroReader.Find(query.TeamId, RetroSessionStateRules.Active, token);
        var retroItems = await _retroReader.GetAll(query.TeamId, token);

        var activeSession = retroSession is not null
            ? RetroSessionConverter.ConvertTo(retroSession)
            : null;
        var itemsResult = retroItems
            .Select(RetroItemConverter.ConvertTo)
            .ToArray();
        
        return new GetRetroStateResult(activeSession, itemsResult);
    }
}