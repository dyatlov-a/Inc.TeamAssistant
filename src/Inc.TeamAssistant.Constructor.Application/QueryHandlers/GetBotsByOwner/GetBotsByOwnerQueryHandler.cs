using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBotsByOwner;

internal sealed class GetBotsByOwnerQueryHandler : IRequestHandler<GetBotsByOwnerQuery, GetBotsByOwnerResult>
{
    private readonly IBotRepository _botRepository;

    public GetBotsByOwnerQueryHandler(IBotRepository botRepository)
    {
        _botRepository = botRepository ?? throw new ArgumentNullException(nameof(botRepository));
    }

    public async Task<GetBotsByOwnerResult> Handle(GetBotsByOwnerQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var bots = await _botRepository.GetBotsByOwner(query.OwnerId, token);
        
        var results = bots
            .Select(b => new BotDto(b.Id, b.Name))
            .ToArray();
        
        return new GetBotsByOwnerResult(results);
    }
}