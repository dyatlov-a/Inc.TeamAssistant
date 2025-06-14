namespace Inc.TeamAssistant.Retro.Application.Contracts;

public sealed class RetroProperties
{
    public long? FacilitatorId { get; set; }
    public Guid? TemplateId { get; set; }
    public TimeSpan? TimerDuration { get; set; }
    public int? VoteCount { get; set; }
}