using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Extensions;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class TeamLinkBuilder : ITeamLinkBuilder
{
    private readonly IQuickResponseCodeGenerator _codeGenerator;
    private readonly IBotAccessor _botAccessor;
    private readonly ITeamAccessor _teamAccessor;

    public TeamLinkBuilder(
        IQuickResponseCodeGenerator codeGenerator,
        IBotAccessor botAccessor,
        ITeamAccessor teamAccessor)
    {
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public string BuildLinkForConnect(string botName, Guid teamId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(botName);
        
        return string.Format(GlobalResources.Settings.LinkForConnectTemplate, botName, teamId.ToLinkSegment());
    }

    public async Task<(string TeamName, string Link, string Code)> GenerateTeamConnector(
        Guid teamId,
        string foreground,
        string background,
        CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);
        
        var teamContext = await _teamAccessor.GetTeamContext(teamId, token);
        var botContext = await _botAccessor.GetBotContext(teamContext.BotId, token);
        var link = BuildLinkForConnect(botContext.UserName, teamId);
        var code = _codeGenerator.Generate(link, foreground, background);
        
        return (teamContext.Name, link, code);
    }
}