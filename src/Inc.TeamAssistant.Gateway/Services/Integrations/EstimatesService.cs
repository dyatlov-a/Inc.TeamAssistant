using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Integrations;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;

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
        var ownerId = integrationContext.TeamProperties.TryGetValue("scrumMaster", out var scrumMaster)
            ? long.Parse(scrumMaster)
            : integrationContext.OwnerId;
        var messageContext = new MessageContext(
            ChatMessage: ChatMessage.Empty,
            Bot: new BotContext(integrationContext.BotId, UserName: string.Empty, new Dictionary<string, string>()),
            Teams: new []
            {
                new TeamContext(
                    integrationContext.TeamId,
                    integrationContext.ChatId,
                    Name: string.Empty,
                    UserInTeam: false,
                    OwnerOfTeam: false)
            },
            Text: string.Empty,
            new Person(ownerId, string.Empty, Username: null),
            LanguageId: LanguageSettings.DefaultLanguageId,
            Location: null,
            TargetPersonId: null,
            ChatName: null);
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