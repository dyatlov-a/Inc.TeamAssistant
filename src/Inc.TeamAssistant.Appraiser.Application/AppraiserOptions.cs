namespace Inc.TeamAssistant.Appraiser.Application;

public sealed class AppraiserOptions
{
	public string ConnectToDashboardLinkTemplate { get; set; } = default!;
	public string[] LinksPrefix { get; set; } = Array.Empty<string>();
}