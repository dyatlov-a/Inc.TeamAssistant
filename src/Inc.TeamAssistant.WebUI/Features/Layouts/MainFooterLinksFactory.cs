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

    public IReadOnlyDictionary<string, IReadOnlyCollection<FooterLink>> Create()
    {
        return new Dictionary<string, IReadOnlyCollection<FooterLink>>
        {
            ["GroupNavigation"] =
            [
                new FooterLink(
                    _localizer["LinkMain"],
                    _navRouter.CreateRoute(null),
                    External: false),
                new FooterLink(
                    _localizer["LinkConstructor"],
                    _navRouter.CreateRoute("constructor"),
                    External: false),
                new FooterLink(
                    _localizer["LinkDashboard"],
                    _navRouter.CreateRoute("dashboard"),
                    External: false)
            ],
            ["GroupTech"] =
            [
                new FooterLink(
                    "DotNet",
                    "https://dotnet.microsoft.com/",
                    External: true),
                new FooterLink(
                    "Blazor",
                    "https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor",
                    External: true),
                new FooterLink(
                    "Nuke",
                    "https://nuke.build/",
                    External: true)
            ],
            ["GroupStorage"] =
            [
                new FooterLink(
                    "Postgres",
                    "https://www.postgresql.org/",
                    External: true),
                new FooterLink(
                    "Npgsql",
                    "https://www.npgsql.org/",
                    External: true),
                new FooterLink(
                    "FluentMigrator",
                    "https://fluentmigrator.github.io/",
                    External: true)
            ],
            ["GroupTools"] =
            [
                new FooterLink(
                    "Telegram.Bot",
                    "https://github.com/TelegramBots/Telegram.Bot",
                    External: true),
                new FooterLink(
                    "FluentValidator",
                    "https://docs.fluentvalidation.net/",
                    External: true),
                new FooterLink(
                    "GitHub",
                    "https://github.com/dyatlov-a/Inc.TeamAssistant",
                    External: true)
            ]
        };
    }
}