using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class TeamLinkBuilder : ITeamLinkBuilder
{
    private readonly IQuickResponseCodeGenerator _codeGenerator;
    private readonly IBotAccessor _botAccessor;
    private readonly ITeamAccessor _teamAccessor;
    private readonly AuthOptions _options;

    public TeamLinkBuilder(
        IQuickResponseCodeGenerator codeGenerator,
        IBotAccessor botAccessor,
        ITeamAccessor teamAccessor,
        AuthOptions options)
    {
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public string BuildLinkForConnect(string botName, Guid teamId)
    {
        return string.Format(_options.LinkForConnectTemplate, botName, teamId.ToString("N"));
    }

    public async Task<(string TeamName, string Link, string Code)> GenerateTeamConnector(
        Guid teamId,
        CancellationToken token)
    {
        var teamContext = await _teamAccessor.GetTeamContext(teamId, token);
        var botContext = await _botAccessor.GetBotContext(teamContext.BotId, token);
        var link = BuildLinkForConnect(botContext.UserName, teamId);
        var code = _codeGenerator.Generate(link);
        
        return (teamContext.TeamName, link, code);
    }
}