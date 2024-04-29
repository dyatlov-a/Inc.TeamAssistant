using System.Reflection;

namespace Inc.TeamAssistant.WebUI.Contracts;

public static class AppVersion
{
    public static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version!.ToString();
}