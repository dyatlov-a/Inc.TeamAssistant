using System.Reflection;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inc.TeamAssistant.Gateway.Pages.Models;

internal sealed class HostPageModel : PageModel
{
    private readonly IMessageProvider _messageProvider;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IClientInfoService _clientInfoService;

    public HostPageModel(
        IMessageProvider messageProvider,
        IWebHostEnvironment webHostEnvironment,
        IClientInfoService clientInfoService)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _clientInfoService = clientInfoService ?? throw new ArgumentNullException(nameof(clientInfoService));
    }

    public LanguageId CurrentLanguageId = default!;
    public string AppVersion = default!;
    public MetaStaticViewModel MetaStaticViewModel = MetaStaticViewModel.Empty;
    public OpenGraphStaticViewModel OpenGraphStaticViewModel = OpenGraphStaticViewModel.Empty;
    public bool HasMap;

    public async Task OnGetAsync()
    {
        var resources = await _messageProvider.Get();
        var currentLanguage = await _clientInfoService.GetCurrentLanguageId();
        var resourcesByLanguage = resources.Result.TryGetValue(currentLanguage.Value, out var data)
            ? data
            : resources.Result[LanguageSettings.DefaultLanguageId.Value];

        CurrentLanguageId = currentLanguage;
        AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        MetaStaticViewModel = new(
            resourcesByLanguage[Messages.MetaTitle.Value],
            resourcesByLanguage[Messages.MetaDescription.Value],
            resourcesByLanguage[Messages.MetaKeywords.Value],
            resourcesByLanguage[Messages.MetaAuthor.Value]);

        HasMap = PageContext.HttpContext.Request.Path.Value?.Contains("/map/") == true;
        OpenGraphStaticViewModel = HasMap
            ? new(
                CurrentLanguageId,
                resourcesByLanguage[CheckInMessages.CheckIn_OgTitle.Value],
                resourcesByLanguage[CheckInMessages.CheckIn_OgDescription.Value],
                "card_checkin")
            : new(
                CurrentLanguageId,
                resourcesByLanguage[Messages.OgTitle.Value],
                resourcesByLanguage[Messages.OgDescription.Value],
                "card_appraiser");
    }

    public bool AnalyticsEnabled() => _webHostEnvironment.IsProduction();
}