using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Integrations;

namespace Inc.TeamAssistant.Gateway.Services.Integrations;

public sealed class EstimatesService
{
    private readonly ICommandExecutor _commandExecutor;
    private readonly IIntegrationsAccessor _integrationsAccessor;
    private readonly ITeamAccessor _teamAccessor;

    public EstimatesService(
        ICommandExecutor commandExecutor,
        IIntegrationsAccessor integrationsAccessor,
        ITeamAccessor teamAccessor)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _integrationsAccessor = integrationsAccessor ?? throw new ArgumentNullException(nameof(integrationsAccessor));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task StartEstimate(StartEstimateRequest request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var integrationContext = await _integrationsAccessor.Find(request.AccessToken, request.ProjectKey, token);
        
        if (integrationContext is null)
            return;
        
        var teammates = await _teamAccessor.GetTeammates(integrationContext.TeamId, DateTimeOffset.UtcNow, token);
        var ownerId = integrationContext.TeamProperties.TryGetValue("scrumMaster", out var scrumMaster) && long.TryParse(scrumMaster, out var value)
            ? value
            : integrationContext.OwnerId;
        var messageContext = MessageContext.CreateFromIntegration(
            integrationContext.BotId,
            integrationContext.TeamId,
            integrationContext.ChatId,
            ownerId);
        var command = new AddStoryCommand(
            messageContext,
            integrationContext.TeamId,
            integrationContext.TeamProperties.GetValueOrDefault("storyType", StoryType.Scrum.ToString()),
            $"[{request.IssueKey}] {request.Subject}",
            new[]
            {
                request.IssueUrl
            },
            teammates);

        await _commandExecutor.Execute(command, token);
    }
}