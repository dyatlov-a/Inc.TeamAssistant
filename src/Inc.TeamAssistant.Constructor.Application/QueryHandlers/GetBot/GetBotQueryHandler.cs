using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Queries.GetBot;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBot;

internal sealed class GetBotQueryHandler : IRequestHandler<GetBotQuery, GetBotResult>
{
    private readonly IBotRepository _repository;
    private readonly IPersonResolver _personResolver;

    public GetBotQueryHandler(IBotRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetBotResult> Handle(GetBotQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var bot = await query.Id.Required(_repository.Find, token);
        
        bot.CheckRights(currentPerson.Id);

        return new GetBotResult(
            bot.Id,
            bot.CalendarId,
            bot.Name,
            bot.Token,
            bot.FeatureIds,
            bot.Properties,
            bot.SupportedLanguages);
    }
}