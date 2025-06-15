namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroProperties
{
    public long? FacilitatorId { get; set; }
    public Guid? TemplateId { get; set; }
    public TimeSpan? TimerDuration { get; set; }
    public int? VoteCount { get; set; }
    public RetroTypes? RetroType { get; set; }

    public RetroTypes RequiredRetroType() => RetroType ?? RetroTypes.Hidden;
}