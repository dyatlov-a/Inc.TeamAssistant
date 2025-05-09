namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class RetroItemViewModel
{
    public Guid Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public long OwnerId { get; set; }
    public int Type { get; set; }
    public string? Text { get; set; }
}