using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Gateway.Services.Integrations;

public sealed class EstimatesService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IntegrationContextProvider _contextProvider;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IBotAccessor _botAccessor;

    public EstimatesService(
        ICommandExecutor commandExecutor,
        IntegrationContextProvider contextProvider,
        ITeamAccessor teamAccessor,
        IBotAccessor botAccessor)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task StartEstimate(StartEstimateRequest request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        var context = _contextProvider.Get();
        var teammates = await _teamAccessor.GetTeammates(context.TeamId, DateTimeOffset.UtcNow, token);
        var ownerId = context.TeamProperties.TryGetValue(ConnectorProperties.ScrumMaster, out var scrumMaster)
                      && long.TryParse(scrumMaster, out var value)
            ? value
            : context.OwnerId;
        var botContext = await _botAccessor.GetBotContext(context.BotId, token);
        var messageContext = MessageContext.CreateFromIntegration(
            botContext,
            context.TeamId,
            context.ChatId,
            ownerId);
        var title = string.IsNullOrWhiteSpace(request.IssueKey)
            ? request.Subject
            : $"[{request.IssueKey}] {request.Subject}";
        var links = string.IsNullOrWhiteSpace(request.IssueUrl)
            ? Array.Empty<string>()
            : [request.IssueUrl];
        var command = new AddStoryCommand(
            messageContext,
            context.TeamId,
            context.TeamProperties.GetStoryType(),
            title,
            links,
            teammates);

        await _commandExecutor.Execute(command, token);
    }
}