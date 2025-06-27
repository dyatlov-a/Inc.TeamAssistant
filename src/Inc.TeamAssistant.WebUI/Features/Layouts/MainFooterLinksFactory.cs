using Inc.TeamAssistant.WebUI.Contracts;
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

    public IReadOnlyDictionary<string, IReadOnlyCollection<MainLinkViewModel>> Create()
    {
        return new Dictionary<string, IReadOnlyCollection<MainLinkViewModel>>
        {
            {
                $"{_localizer["GroupNavigation"]} {ApplicationContext.GetVersion()}",
                [
                    new MainLinkViewModel(
                        _localizer["LinkMain"],
                        _navRouter.CreateRoute(null),
                        External: false),
                    new MainLinkViewModel(
                        _localizer["LinkConstructor"],
                        _navRouter.CreateRoute("constructor"),
                        External: false),
                    new MainLinkViewModel(
                        _localizer["LinkDashboard"],
                        _navRouter.CreateRoute("dashboard"),
                        External: false)
                ]
            },
            {
                _localizer["GroupTech"],
                [
                    new MainLinkViewModel(
                        "DotNet",
                        "https://dotnet.microsoft.com/",
                        External: true),
                    new MainLinkViewModel(
                        "Blazor",
                        "https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor",
                        External: true),
                    new MainLinkViewModel(
                        "Nuke",
                        "https://nuke.build/",
                        External: true)
                ]
            },
            {
                _localizer["GroupStorage"],
                [
                    new MainLinkViewModel(
                        "Postgres",
                        "https://www.postgresql.org/",
                        External: true),
                    new MainLinkViewModel(
                        "Npgsql",
                        "https://www.npgsql.org/",
                        External: true),
                    new MainLinkViewModel(
                        "FluentMigrator",
                        "https://fluentmigrator.github.io/",
                        External: true)
                ]
            },
            {
                _localizer["GroupTools"],
                [
                    new MainLinkViewModel(
                        "Telegram.Bot",
                        "https://github.com/TelegramBots/Telegram.Bot",
                        External: true),
                    new MainLinkViewModel(
                        "FluentValidator",
                        "https://docs.fluentvalidation.net/",
                        External: true),
                    new MainLinkViewModel(
                        "GitHub",
                        "https://github.com/dyatlov-a/Inc.TeamAssistant",
                        External: true)
                ]
            }
        };
    }
}