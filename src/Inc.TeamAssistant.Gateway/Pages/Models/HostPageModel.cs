using System.Reflection;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Primitives.Languages;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inc.TeamAssistant.Gateway.Pages.Models;

internal sealed class HostPageModel : PageModel
{
    private readonly IMessageProvider _messageProvider;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILanguageProvider _languageProvider;

    public HostPageModel(
        IMessageProvider messageProvider,
        IWebHostEnvironment webHostEnvironment,
        ILanguageProvider languageProvider)
    {
        _messageProvider = messageProvider ?? throw new ArgumentNullException(nameof(messageProvider));
        _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        _languageProvider = languageProvider ?? throw new ArgumentNullException(nameof(languageProvider));
    }

    public LanguageId CurrentLanguageId = default!;
    public string AppVersion = default!;
    public MetaStaticViewModel MetaStaticViewModel = MetaStaticViewModel.Empty;
    public OpenGraphStaticViewModel OpenGraphStaticViewModel = OpenGraphStaticViewModel.Empty;
    public bool HasMap;

    public async Task OnGetAsync()
    {
        var resources = await _messageProvider.Get();
        var currentLanguage = _languageProvider.GetCurrentLanguageId();
        var resourcesByLanguage = resources.Result.TryGetValue(currentLanguage.Language.Value, out var data)
            ? data
            : resources.Result[LanguageSettings.DefaultLanguageId.Value];

        CurrentLanguageId = currentLanguage.Language;
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