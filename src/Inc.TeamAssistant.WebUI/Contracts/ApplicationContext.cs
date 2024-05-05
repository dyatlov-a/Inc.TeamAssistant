using System.Reflection;

namespace Inc.TeamAssistant.WebUI.Contracts;

public static class ApplicationContext
{
    public static readonly string AuthenticationScheme = "Cookies";
    
    public static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version!.ToString();
}