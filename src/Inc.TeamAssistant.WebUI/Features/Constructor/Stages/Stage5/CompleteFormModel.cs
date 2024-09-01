using System.Text;
using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage5;

public sealed class CompleteFormModel
{
    public string UserName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public IReadOnlyCollection<Guid> FeatureIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public IReadOnlyCollection<string> SupportedLanguages { get; set; } = Array.Empty<string>();
    public IReadOnlyCollection<BotDetailsFormModel> BotDetails { get; set; } = Array.Empty<BotDetailsFormModel>();
    public WorkScheduleUtcDto? WorkSchedule { get; set; }
    public IReadOnlyCollection<DayOfWeek> Weekends { get; set; } = Array.Empty<DayOfWeek>();
    public IReadOnlyDictionary<DateOnly, string> Holidays { get; set; } = new Dictionary<DateOnly, string>();
    public string HolidaysAsText { get; set; } = string.Empty;
    
    public string GetWorkScheduleAsText(string workAllDay)
    {
        var builder = new StringBuilder();

        if (WorkSchedule is null)
            builder.Append(workAllDay);
        else
            builder.Append($"{WorkSchedule.Start:HH:mm} - {WorkSchedule.End:HH:mm}");

        return builder.ToString();
    }

    public CompleteFormModel Apply(StagesState stagesState)
    {
        ArgumentNullException.ThrowIfNull(stagesState);

        UserName = stagesState.UserName;
        Token = stagesState.Token;
        FeatureIds = stagesState.FeatureIds.ToArray();
        Properties = stagesState.SelectedFeatures
            .SelectMany(f => f.Properties.Select(p => (
                Key: p,
                Value: stagesState.Properties.GetValueOrDefault(p, string.Empty))))
            .ToDictionary(f => f.Key, f => f.Value, StringComparer.InvariantCultureIgnoreCase);
        SupportedLanguages = stagesState.SupportedLanguages.ToArray();
        BotDetails = stagesState.BotDetails.Select(b => new BotDetailsFormModel
        {
            LanguageId = b.LanguageId,
            Name = b.Name,
            ShortDescription = b.ShortDescription,
            Description = b.Description
        }).ToArray();
        WorkSchedule = stagesState.Calendar.WorkAllDay
            ? null
            : new(stagesState.Calendar.Schedule.Start, stagesState.Calendar.Schedule.End);
        Weekends = stagesState.Calendar.Weekends.ToArray();
        Holidays = stagesState.Calendar.Holidays.ToDictionary();
        HolidaysAsText = Convert(stagesState.Calendar.Holidays);

        return this;
    }
    
    private string Convert(IReadOnlyDictionary<DateOnly, string> holidays)
    {
        ArgumentNullException.ThrowIfNull(holidays);
        
        return holidays.Any()
            ? $"{string.Join(", ", holidays.Select(h => h.Key.ToString("dd.MM.yyyy")))} ({holidays.Count})"
            : "-";
    }
}