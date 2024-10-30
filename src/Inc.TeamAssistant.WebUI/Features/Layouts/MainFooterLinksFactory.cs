using Inc.TeamAssistant.WebUI.Routing;
using Microsoft.Extensions.Localization;

namespace Inc.TeamAssistant.WebUI.Features.Layouts;

internal sealed class MainFooterLinksFactory
{
    private readonly IStringLocalizer<LayoutResources> _localizer;
    private readonly NavRouter _navRouter;

    public MainFooterLinksFactory(IStringLocalizer<LayoutResources> localizer, NavRouter navRouter)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _navRouter = navRouter ?? throw new ArgumentNullException(nameof(navRouter));
    }

    public IReadOnlyDictionary<string, IReadOnlyCollection<LinkModel>> Create()
    {
        return new Dictionary<string, IReadOnlyCollection<LinkModel>>
        {
            ["GroupNavigation"] =
            [
                new LinkModel(
                    _localizer["LinkMain"],
                    _navRouter.CreateRoute(null),
                    External: false),
                new LinkModel(
                    _localizer["LinkConstructor"],
                    _navRouter.CreateRoute("constructor"),
                    External: false),
                new LinkModel(
                    _localizer["LinkDashboard"],
                    _navRouter.CreateRoute("dashboard"),
                    External: false)
            ],
            ["GroupTech"] =
            [
                new LinkModel(
                    "DotNet",
                    "https://dotnet.microsoft.com/",
                    External: true),
                new LinkModel(
                    "Blazor",
                    "https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor",
                    External: true),
                new LinkModel(
                    "Nuke",
                    "https://nuke.build/",
                    External: true)
            ],
            ["GroupStorage"] =
            [
                new LinkModel(
                    "Postgres",
                    "https://www.postgresql.org/",
                    External: true),
                new LinkModel(
                    "Npgsql",
                    "https://www.npgsql.org/",
                    External: true),
                new LinkModel(
                    "FluentMigrator",
                    "https://fluentmigrator.github.io/",
                    External: true)
            ],
            ["GroupTools"] =
            [
                new LinkModel(
                    "Telegram.Bot",
                    "https://github.com/TelegramBots/Telegram.Bot",
                    External: true),
                new LinkModel(
                    "FluentValidator",
                    "https://docs.fluentvalidation.net/",
                    External: true),
                new LinkModel(
                    "GitHub",
                    "https://github.com/dyatlov-a/Inc.TeamAssistant",
                    External: true)
            ]
        };
    }
}