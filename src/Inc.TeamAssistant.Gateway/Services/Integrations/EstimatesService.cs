using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Gateway.Services.Integrations;

public sealed class EstimatesService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IntegrationContextProvider _integrationContextProvider;
    private readonly ITeamAccessor _teamAccessor;

    public EstimatesService(
        ICommandExecutor commandExecutor,
        IntegrationContextProvider integrationContextProvider,
        ITeamAccessor teamAccessor)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _integrationContextProvider = integrationContextProvider ?? throw new ArgumentNullException(nameof(integrationContextProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task StartEstimate(StartEstimateRequest request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var integrationContext = _integrationContextProvider.Get();
        
        var teammates = await _teamAccessor.GetTeammates(integrationContext.TeamId, DateTimeOffset.UtcNow, token);
        var ownerId = integrationContext.TeamProperties.TryGetValue("scrumMaster", out var scrumMaster) && long.TryParse(scrumMaster, out var value)
            ? value
            : integrationContext.OwnerId;
        var messageContext = MessageContext.CreateFromIntegration(
            integrationContext.BotId,
            integrationContext.TeamId,
            integrationContext.ChatId,
            ownerId);
        var title = string.IsNullOrWhiteSpace(request.IssueKey)
            ? request.Subject
            : $"[{request.IssueKey}] {request.Subject}";
        var links = string.IsNullOrWhiteSpace(request.IssueUrl)
            ? Array.Empty<string>()
            : [request.IssueUrl];
        var command = new AddStoryCommand(
            messageContext,
            integrationContext.TeamId,
            integrationContext.TeamProperties.GetValueOrDefault("storyType", StoryType.Fibonacci.ToString()),
            title,
            links,
            teammates);

        await _commandExecutor.Execute(command, token);
    }
}